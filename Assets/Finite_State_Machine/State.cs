using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE
    {
        IDLE, ATTACK, PATROL, PUSUE, SLEEP, SAFE
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected NavMeshAgent agent;
    protected Transform player;
    protected Animator animation;
    protected State nextState;

    public float visDist = 10f;
    public float vidAngle = 30f;
    public float shootDist = 7f;
    public float safeDist = 2f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        animation = _anim;
        player = _player;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }


    public State Process()
    {
        if (stage == EVENT.ENTER)
            Enter();
        if (stage == EVENT.UPDATE)
            Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public bool CanSeePlayer()
    {
        Vector3 v = player.position - npc.transform.position;
        float angle = Vector3.Angle(v, npc.transform.forward);

        if (angle < vidAngle && v.magnitude < visDist)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 v = player.position - npc.transform.position;
        if (v.magnitude <= shootDist)
            return true;

        return false;
    }

    public bool CanGoToSafe()
    {
        Vector3 v = player.position - npc.transform.position;
        float angle = Vector3.Angle(v, -npc.transform.forward);

        if (angle < vidAngle && v.magnitude < safeDist)
            return true;

        return false;
    }
}


public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) :
                base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        animation.SetTrigger("isIdle");
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, animation, player);
            stage = EVENT.EXIT;
        }
        else if (Random.Range(0, 100) < 10)
        {
            nextState = new Patrol(npc, agent, animation, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        animation.ResetTrigger("isIdle");
        base.Exit();
    }
}

public class Patrol : State
{
    private int currIndex;

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) :
                base(_npc, _agent, _anim, _player)
    {
        name = STATE.PATROL;
        agent.speed = 3;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        float lastDist = Mathf.Infinity;
        currIndex = -1;
        for (int i = 0; i < GameEnviroment.Singleton.CheckPoints.Count; i++)
        {
            float distance = (player.position - npc.transform.position).magnitude;

            if (distance < lastDist)
            {
                currIndex = i;
                lastDist = distance;
            }
        }
        animation.SetTrigger("isWalking");
        base.Enter();
    }

    public override void Update()
    {
        if (CanGoToSafe())
        {
            nextState = new Safe(npc, agent, animation, player);
            stage = EVENT.EXIT;
        }
        else
        {
            if (CanSeePlayer())
            {
                nextState = new Pursue(npc, agent, animation, player);
                stage = EVENT.EXIT;
            }
            if (agent.remainingDistance < 1f)
            {
                if (currIndex >= GameEnviroment.Singleton.CheckPoints.Count - 1)
                    currIndex = 0;
                else
                    currIndex++;

                agent.SetDestination(GameEnviroment.Singleton.CheckPoints[currIndex].transform.position);
            }
        }
    }

    public override void Exit()
    {
        animation.ResetTrigger("isWalking");
        base.Exit();
    }
}

public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) :
               base(_npc, _agent, _anim, _player)
    {
        name = STATE.PUSUE;
        agent.speed = 6;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        animation.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        agent.SetDestination(player.position);
        if (agent.hasPath)
        {
            if (CanAttackPlayer())
            {
                nextState = new Attack(npc, agent, animation, player);
                stage = EVENT.EXIT;
            }
            else if (!CanSeePlayer())
            {
                nextState = new Idle(npc, agent, animation, player);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        animation.ResetTrigger("isRunning");
        base.Exit();
    }
}

public class Attack : State
{
    float rotSpeed = 5f;
    AudioSource audio;

    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) :
               base(_npc, _agent, _anim, _player)
    {
        name = STATE.ATTACK;
        audio = _npc.GetComponent<AudioSource>();
    }

    public override void Enter()
    {
        agent.isStopped = true;
        animation.SetTrigger("isShooting");
        audio.Play();
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        direction.y = 0;
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
                        Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

        if (!CanAttackPlayer())
        {
            nextState = new Idle(npc, agent, animation, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        animation.ResetTrigger("isShooting");
        audio.Stop();
        base.Exit();
    }
}

public class Safe : State
{
    GameObject safePlace;
    public Safe(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) :
               base(_npc, _agent, _anim, _player)
    {
        name = STATE.SAFE;
        agent.speed = 6;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        safePlace = GameObject.FindGameObjectWithTag("Safe");
        agent.SetDestination(safePlace.transform.position);
        animation.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        if (agent.hasPath)
        {
            if (agent.remainingDistance < 1f)
            {
                nextState = new Idle(npc, agent, animation, player);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        animation.ResetTrigger("isRunning");
        base.Exit();
    }
}