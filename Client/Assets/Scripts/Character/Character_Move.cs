using UnityEngine;

public partial class Character : MonoBehaviour
{
    protected void OnInitMove()
	{
        SyncMove = new CharactorReplay(this);
        mDirection = transform.forward;
		mLogicPosition = transform.position;

        //Vector3 p = mLogicPosition;
        //p.y += 1.0f;
        // NavLayer = NavigationSystem.GetLayer(mLogicPosition);
        NavLayer = 1;
	}

	protected void UpdateMove()
	{
        if (SyncMove == null)
            return;
        SyncMove.LogicTick();

        if (mDirectionChg)
		{
			transform.forward = mDirection;
			mDirectionChg = false;
		}
		if (!IsCannotFlag(CannotFlag.CannotMove) && MoveSpeed > 0.0f)
		{
			Vector3 deltaPos = mSpeed * Time.fixedDeltaTime * mDirection;
			deltaPos.y = 0.0f;
            mLogicPosition += deltaPos;
            
            Vector3 edgePoint0, edgePoint1;
			Vector3 hitPoint;
            if (NavigationSystem.LineCastEdge(transform.position, mLogicPosition, NavLayer, out hitPoint, out edgePoint0, out edgePoint1))
            {
                Vector3 dir0 = edgePoint1 - edgePoint0;
                dir0.y = 0.0f;
                dir0.Normalize();
                Vector3 dir1 = -dir0;
                Vector3 dir3 = transform.forward;
                dir3.Normalize();

                float cos0 = Vector3.Dot(dir0, dir3);
                float cos1 = Vector3.Dot(dir1, dir3);

                if (cos0 > cos1)
                {
                    mLogicPosition.x = transform.position.x + Time.fixedDeltaTime * MoveSpeed * 0.5f * dir0.x;
                    mLogicPosition.z = transform.position.z + Time.fixedDeltaTime * MoveSpeed * 0.5f * dir0.z;
                }
                else
                {
                    mLogicPosition.x = transform.position.x + Time.fixedDeltaTime * MoveSpeed * 0.5f * dir1.x;
                    mLogicPosition.z = transform.position.z + Time.fixedDeltaTime * MoveSpeed * 0.5f * dir1.z;
                }
                if (!NavigationSystem.LineTest(transform.position, mLogicPosition, NavLayer))
                {
                    float height;
                    if (NavigationSystem.GetLayerHeight(mLogicPosition, NavLayer, out height))
                    {
                        mLogicPosition.y = height;
                    }
                    transform.position = mLogicPosition;
                }
            }
            else
            {
                float height;
                if (NavigationSystem.GetLayerHeight(mLogicPosition, NavLayer, out height))
                {
                    mLogicPosition.y = height;
                }
                transform.position = mLogicPosition;
            }

            mLogicPosition = transform.position;
			// transform.position = mLogicPosition;
		}

        UpdateSearchMove();
    }

    public bool IsSearchMoving()
    {
        return mPath != null;
    }

    void UpdateSearchMove()
    {
        if (mPath == null || mPath.Length == 0)
            return;
        Vector3 nextPos = mPath[mCurrentNodeIndex];
        Vector3 dir = nextPos - Position;
        if (dir.magnitude <= 0.1f)
        {
            ++mCurrentNodeIndex;
            if (mCurrentNodeIndex >= mPath.Length)
            {
                Direction = dir;
                StopSearchMove();
            }
            return;
        }
        Vector3 targetPos = mPath[mPath.Length - 1];
        if (Vector3.Distance(Position, targetPos) <= mDestRadius)
        {
            Direction = dir;
            StopSearchMove();
            return;
        }
        Direction = dir;
        MoveSpeed = 3.0f;
    }

    public void SearchMove(Vector3 pos, float destRadius = 0.3f, bool sync = true)
    {
        if (NavLayer == 0)
        {
            NavLayer = NavigationSystem.GetLayer(pos);
            if (NavLayer == 0)
                return;
        }
        Vector3[] path;
        if (!NavigationSystem.Nav_CalcLayerPath(Position, pos, NavLayer, out path))
        {
            return;
        }
        mPath = path;
        mCurrentNodeIndex = 0;
        MoveSpeed = 0.0f;
        mDestRadius = destRadius;
    }

    public void LineMove(Vector3 pos, float destRadius = 0.3f, bool sync = true)
    {
        Vector3 destPos;
        if (!NavigationSystem.LineCast(Position, pos, NavLayer, out destPos))
        {
            return;
        }
        mPath = new Vector3[1] { destPos };
        mCurrentNodeIndex = 0;
        MoveSpeed = 0.0f;
        mDestRadius = destRadius;
    }

    public void StopMove()
    {
        MoveSpeed = 0.0f;
        if (IsSearchMoving())
        {
            StopSearchMove();
        }
    }

    public void StopSearchMove()
    {
        MoveSpeed = 0.0f;
        mPath = null;
        mCurrentNodeIndex = 0;
    }

    protected void UpdateAnim()
    {
        if (MoveSpeed > 0.0f)
        {
            PlayAnimation(2, AnimPlayType.Base, 1.0f, true);
        }
        else
        {
            PlayAnimation(1, AnimPlayType.Base, 1.0f, true);
        }
    }

	public Vector3 Direction
	{
		set
		{
			if (mDirection == value)
				return;
            Vector3 v = value;
            v.y = 0.0f;
            v.Normalize();
            float dot = Vector3.Dot(v, mDirection);
            if (dot > 0.98f)
                return;

            mDirection = v;
			if (mDirection != Vector3.zero)
			{
				transform.forward = mDirection;
			}
			mDirectionChg = true;
		}
		get
		{
			return mDirection;
		}
	}

	public float MoveSpeed
	{
		set
		{
			mSpeed = value;
		}
		get
		{
			return mSpeed;
		}
	}

    public Vector3 Position
    {
        set
        {
            transform.position = value;
            mLogicPosition = value;
        }
        get
        {
            return transform.position;
        }
    }

    public uint NavLayer
    {
        private set;
        get;
    }

    public CharactorReplay SyncMove
    {
        private set;
        get;
    }

    float mSpeed = 0.0f;
	Vector3 mDirection = Vector3.forward;
	bool mDirectionChg = false;
	Vector3 mLogicPosition = Vector3.zero;

    // Search Path
    private Vector3[] mPath;
    private int mCurrentNodeIndex = 0;
    private float mDestRadius = 0.1f;

}