using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse.Utility
{
    public class CardAlignmentHelper
    {
        internal CardAlignmentHelper(Card _card, CardController _controller = null)
        {
            card = _card;
            controller = _controller;
        }

        internal CardAlignmentHelper(TurnTaker _turntaker, CardController _controller = null)
        {
            turntaker = _turntaker;
            controller = _controller;
        }

        public CardAlignmentHelper Target()
        {
            target = CardTarget.Target;
            return this;
        }

        public CardAlignmentHelper NonTarget()
        {
            target = CardTarget.Nontarget;
            return this;
        }

        public CardAlignmentHelper Character()
        {
            character = true;
            return this;
        }

        public CardAlignmentHelper Noncharacter()
        {
            character = false;
            return this;
        }

        public CardAlignmentHelper Hero()
        {
            alignment = CardAlignment.Hero;
            return this;
        }

        public CardAlignmentHelper Environment()
        {
            alignment = CardAlignment.Environment;
            return this;
        }

        public CardAlignmentHelper Villain()
        {
            alignment = CardAlignment.Villain;
            return this;
        }

        public CardAlignmentHelper NonHero()
        {
            alignment = CardAlignment.Nonhero;
            return this;
        }

        public CardAlignmentHelper NonEnvironment()
        {
            alignment = CardAlignment.Nonenvironment;
            return this;
        }

        public CardAlignmentHelper NonVillain()
        {
            alignment = CardAlignment.Nonvillain;
            return this;
        }

        public static implicit operator bool(CardAlignmentHelper helper)
        {
            if (! helper.alignment.HasValue)
            {
                throw new InvalidOperationException("CardAlignmentHelper without alignment converted to bool");
            }

            if (helper.card != null)
            {
                return helper.controller.HasAlignment(helper.card, helper.alignment.Value, helper.target) &&
                    (helper.character == null || helper.character.Value == helper.card.IsCharacter);
            }

            if (helper.turntaker != null)
            {
                if (helper.target == CardTarget.Target)
                {
                    return false;
                }

                return helper.controller.HasAlignment(helper.turntaker, helper.alignment.Value);
            }

            throw new InvalidOperationException("CardAlignmentHelper without card or turntaker converted to bool");
        }

        private TurnTaker turntaker;
        private Card card;
        private CardController controller;

        private CardAlignment? alignment;
        private CardTarget target = CardTarget.Either;
        private bool? character = null;
    }
}
