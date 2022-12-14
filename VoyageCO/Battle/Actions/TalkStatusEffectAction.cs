using VCO.Utility;

namespace VCO.Battle.Actions
{
    internal class TalkStatusEffectAction : StatusEffectAction
    {
        public TalkStatusEffectAction(ActionParams aparams) : base(aparams)
        {
            string text = string.Empty;
            this.controller.Data.TryGetValue("topicOfDiscussion", out object obj);
            if (obj is string)
            {
                text = (obj as string);
            }
            if (this.senderArticle.Length > 0)
            {
                this.senderArticle = Capitalizer.Capitalize(this.senderArticle);
            }
            else if (this.senderName.Length > 0)
            {
                this.senderName = Capitalizer.Capitalize(this.senderName);
            }
            if (this.effect.TurnsRemaining > 1)
            {
                this.message = string.Format("@{0}{1} is busy talking about {2} with {3}{4}.", new object[]
                {
                    this.senderArticle,
                    this.senderName,
                    text,
                    this.targetArticle,
                    this.targetName
                });
            }
            else
            {
                this.message = string.Format("@{0}{1} got bored of talking about {2} with {3}{4}.", new object[]
                {
                    this.senderArticle,
                    this.senderName,
                    text,
                    this.targetArticle,
                    this.targetName
                });
            }
            this.actionStartSound = this.controller.InterfaceController.TalkSound;
        }

        private const string TOPIC_OF_DISCUSSION = "topicOfDiscussion";
    }
}
