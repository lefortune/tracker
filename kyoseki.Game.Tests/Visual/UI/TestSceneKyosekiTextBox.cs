using kyoseki.Game.UI.Input;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.UI
{
    public class TestSceneKyosekiTextBox : TestScene
    {
        private KyosekiTextBox textBox;

        public TestSceneKyosekiTextBox()
        {
            Add(new TooltipContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        textBox = new KyosekiTextBox
                        {
                            RelativeSizeAxes = Axes.X,
                            PlaceholderText = "Type something here"
                        },
                        new TestATextBox
                        {
                            RelativeSizeAxes = Axes.X,
                            PlaceholderText = "Type a here"
                        },
                        new ButtonTextBox
                        {
                            RelativeSizeAxes = Axes.X,
                            PlaceholderText = "I have buttons",
                            Buttons = new ButtonInfo[]
                            {
                                new ButtonInfo(FontAwesome.Solid.Trash, "Delete"),
                                new ButtonInfo(FontAwesome.Solid.Undo, "Undo"),
                                new ButtonInfo(FontAwesome.Brands.Twitter, "Cancer")
                            }
                        }
                    }
                }
            });

            AddStep("toggle readonly", () => textBox.ReadOnly.Value = !textBox.ReadOnly.Value);
        }

        private class TestATextBox : KyosekiTextBox
        {
            protected override bool CanAddCharacter(char character) => character == 'a';
        }
    }
}