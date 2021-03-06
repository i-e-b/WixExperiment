﻿namespace OptionalPlugin
{
    using System.ComponentModel.Composition;
    using PluginContract;

    [Export(typeof(IPluginContract))]
    public class FriendlyPlugin : IPluginContract
    {
        public string Greeting()
        {
            return "Hello from optional plugin!";
        }
    }
}
