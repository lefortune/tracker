﻿using System;
using System.Collections.Generic;
using System.Linq;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using kyoseki.Game.UI.SerialMonitor;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace kyoseki.Game.Overlays
{
    public class SerialMonitorOverlay : SlideInOverlay
    {
        private ConnectionManager serialConnections;

        private SerialTabControl tabControl;

        private List<SerialChannel> loadedChannels = new List<SerialChannel>();

        private Container tabContent;

        [BackgroundDependencyLoader]
        private void load(ConnectionManager serialConnections)
        {
            this.serialConnections = serialConnections;

            Children = new Drawable[]
            {
                new Box
                {
                    Colour = KyosekiColors.BACKGROUND.Darken(0.2f).Opacity(0.5f),
                    RelativeSizeAxes = Axes.Both
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding
                    {
                        Top = SerialTabControl.HEIGHT
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = KyosekiColors.BACKGROUND.Opacity(0.5f)
                        },
                        tabContent = new Container { RelativeSizeAxes = Axes.Both }
                    }
                },
                tabControl = new SerialTabControl()
            };

            handlePortsUpdated(serialConnections.PortNames.ToArray(), Array.Empty<string>());

            serialConnections.PortsUpdated += handlePortsUpdated;
            serialConnections.MessageReceived += handleMessage;
            tabControl.Current.ValueChanged += handleTabChanged;
        }

        private void handleTabChanged(ValueChangedEvent<string> e)
        {
            var tab = loadedChannels.Find(c => c.Port == e.NewValue);
            tabContent.FadeOut(500, Easing.OutQuint);
            LoadComponentAsync(tab, t =>
            {
                tabContent.Clear(false);
                tabContent.Add(t);
                tabContent.FadeIn(500, Easing.In);
            });
        }

        private void handlePortsUpdated(string[] added, string[] removed) => Schedule(() =>
        {
            foreach (var port in added)
            {
                if (!loadedChannels.Any(c => c.Port == port))
                {
                    tabControl.AddItem(port);
                    loadedChannels.Add(new SerialChannel(port)
                    {
                        RelativeSizeAxes = Axes.Both
                    });
                }
            }
        });

        private void handleMessage(MessageInfo msg) => Schedule(() =>
        {
            var channel = loadedChannels.Find(c => c.Port == msg.Port);
            channel.AddMessage(msg);
        });

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.PortsUpdated -= handlePortsUpdated;
            serialConnections.MessageReceived -= handleMessage;
        }
    }
}
