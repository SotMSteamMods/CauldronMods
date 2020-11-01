namespace Cauldron.Baccarat
{
    using System;
    using System.Collections;

    using Handelabra.Sentinels.Engine.Model;

    public class BaccaratCharacterCardController : HeroCharacterCardController
    {
        #region Constructors

        public BaccaratCharacterCardController(Card card, TurnTakercontroller turnTakercontroller)
            : base(card, turnTakercontroller)
        {

        }

        #endregion Constructors

        #region Properties

        private ITrigger ReduceTrigger
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //Put 2 cards from a trash on the bottom of their deck.
                        break;
                    }
                case 1:
                    {
                        //Increase the next damage dealt by a hero target by 2.
                        break;
                    }
                case 2:
                    {
                        //Each hero character may deal themselves 3 toxic damage to use a power now.
                        break;
                    }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard the top card of your deck or put up to 2 trick cards with the same name from your trash into play.
        }

        #endregion Methods
    }
}