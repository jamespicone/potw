using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class StrangerAndMasterProtocolsCardController : CoilsBaseSelfDestructCardController
    {
        public StrangerAndMasterProtocolsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);

            // Much of this borrowed from Isolated Hero. Not sure why visibility isn't sufficient; this will
            // do for now.
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource source)
        {
            if (card.Is(this).NonHero()) { return null; }
            return AskIfTurnTakerIsVisibleToCardSource(card.Owner, source);
        }

        public override bool? AskIfTurnTakerIsVisibleToCardSource(TurnTaker tt, CardSource cardSource)
        {
            if (tt.Is(this).NonHero()) { return null; }
            if (cardSource?.Card.Is(this).NonHero()) { return null; }
            if (cardSource.Card.Owner == tt) { return null; }

            return false;
        }

        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            foreach (var hero1 in Game.HeroTurnTakers)
            {
                foreach (var hero2 in Game.HeroTurnTakers)
                {
                    if (hero1 == hero2) { continue; }

                    if (
                        (gameAction.DoesFirstCardAffectSecondCard(c => c.Owner == hero1 && c.Is(this).Hero(), c => c.Owner == hero2 && c.Is(this).Hero()) ?? false) ||
                        (gameAction.DoesFirstTurnTakerAffectSecondTurnTaker(tt => tt == hero1, tt => tt == hero2) ?? false))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private IEnumerator RemoveDecisions(MakeDecisionsAction mda)
        {
            mda.RemoveDecisions(d => d.CardSource.Card.Owner != d.HeroTurnTakerController.TurnTaker);
            yield break;
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddTrigger<MakeDecisionsAction>(
                mda => mda.CardSource?.Card.Is(this).Hero(),
                mda => RemoveDecisions(mda),
                TriggerType.RemoveDecision,
                TriggerTiming.Before
            );
        }
    }
}
