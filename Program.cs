using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
        }

        public void Save()
        {
            
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }


public void Main(string argument, UpdateType updateSource)
{
    var batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries);

    double totalStored = 0;
    double totalCapacity = 0;

    foreach (var battery in batteries)
    {
        
        string[] lines = battery.DetailedInfo.Split('\n');
        foreach (var line in lines)
        {
            if (line.StartsWith("Stored power:", StringComparison.OrdinalIgnoreCase))
            {
                // Example: "Stored power: 2.50 MWh / 3.00 MWh"
                string[] parts = line.Substring("Stored power:".Length).Trim().Split('/');
                if (parts.Length == 2)
                {
                    double current = ParsePower(parts[0]);
                    double max = ParsePower(parts[1]);
                    totalStored += current;
                    totalCapacity += max;
                }
            }
        }
    }

    double percent = totalCapacity > 0 ? (totalStored / totalCapacity * 100.0) : 0;

    string output = "Battery Status\n";
    output += "----------------------\n";
    output += "Stored: " + totalStored.ToString("F2") + " MWh\n";
    output += "Max:    " + totalCapacity.ToString("F2") + " MWh\n";
    output += "Charge: " + percent.ToString("F1") + "%";

    Echo(output);

    // Now find LCDs manually and check their names
    var lcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);

    for (int i = 0; i < lcds.Count; i++)
    {
        var panel = lcds[i];
        if (panel.CustomName.Contains("Battery Status"))
        {
            panel.ContentType = ContentType.TEXT_AND_IMAGE;
            panel.WriteText(output);
        }
    }
}

// Helper function to parse "2.50 MWh" or "500 kWh" into a double (MWh)
double ParsePower(string input)
{
    input = input.Trim().ToUpper();
    double multiplier = 1.0;

    if (input.EndsWith("MWH"))
    {
        input = input.Replace("MWH", "").Trim();
        multiplier = 1.0;
    }
    else if (input.EndsWith("KWH"))
    {
        input = input.Replace("KWH", "").Trim();
        multiplier = 0.001;
    }

    double value;
    if (double.TryParse(input, out value))
    {
        return value * multiplier;
    }

    return 0;
}

    }
}
