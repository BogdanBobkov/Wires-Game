using Internals;
using Inputs;
using System;

namespace Controllers
{
    public interface IGameView : IRegistrable, ITouchable
    {
        void ClearWindow();
        void CreateWalls(int numWalls);

        event Action OnWallsConnected;
    }
}
