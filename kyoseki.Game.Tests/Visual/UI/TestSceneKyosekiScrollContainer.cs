using kyoseki.Game.UI;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.UI
{
    public class TestSceneKyosekiScrollContainer : TestScene
    {
        private KyosekiScrollContainer scroll;

        protected override double TimePerAction => 50;

        public TestSceneKyosekiScrollContainer()
        {
            const int item_height = 30;

            Add(scroll = new KyosekiScrollContainer
            {
                RelativeSizeAxes = Axes.Both
            });

            int y = 0;

            AddRepeatStep("add items", () =>
            {
                scroll.Add(new SpriteText
                {
                    RelativeSizeAxes = Axes.X,
                    Font = new FontUsage(size: item_height),
                    Text = "hello there!",
                    Y = y
                });

                y += item_height + 5;
            }, 100);
        }
    }
}