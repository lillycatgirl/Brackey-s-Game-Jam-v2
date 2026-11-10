namespace Gameplay.Game
{
    [System.Serializable]
    public class StoryState
    {

        public bool gameStarted;


        public bool seenMonster;
        public bool hasBeenCaught;

        public bool monsterFirstEncounter;
        public bool monsterSecondEncounter;

        public bool talkedToMonster;
        public bool followingMonster;


        public bool insane;


        public bool alone;

        public bool girlDead;
        public bool oldManDead;


        public bool seenFirstTile;
        public bool seenFirstFood;

        public int tileCount;
        public int foodCount;


        public bool girlAskedForFood;
        public bool gaveGirlFood;


        public bool monsterDealOffered;
        public bool acceptedMonsterDeal;
        public bool declinedMonsterDeal;


        public bool foundExit;
        public bool exitActivated;

        public bool stoleTiles;
        public bool decidedToFindMoreTiles;

        public bool playerBetrayedGroup;
    }
}