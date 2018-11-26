using System;
using System.Collections.Generic;

public class Scene
{
    public void Enter()
    {
        if (mScnLists.temp > 0)
        {
            GameController.OnLoadScene();
        }
    }

    public void Exit()
    {

    }

    public excel_scn_list mScnLists = null;
}
