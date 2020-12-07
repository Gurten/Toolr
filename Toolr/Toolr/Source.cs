

//using NSubstitute;
using Parsel.Cache.Core;
using Parsel.Cache.MccReach.Context;
using Parsel.Schema.MccHalo3.Phmo;
using Parsel.Schema.MccReach.Phmo;
using Parsel.Schema.Phmo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toolr.Details;

class Program
{
    enum CommandResult
    { 
        Success,
        Failed,
        MoreInformationRequired
    }

    interface ICommandLineOperation
    {
        string CommandIdentifier { get; }
        string Usage { get; }
        string Description { get; }

        CommandResult Invoke(string[] args);
    }

    class PhysicsModelCommand : ICommandLineOperation
    {
        public string Usage => CommandIdentifier + " <" + RequiredFirstOptions() + "> <input uri> <output uri>";

        public string CommandIdentifier => "physics-model";

        public string Description => "Creates a phmo tag from input.\n" + 
            "Input is structured JSON from the Blender PHMO exporter.\n" +
            "Output is a TagContainer that can be imported into Assembly.\n";

        Dictionary<string, Action<string[]>> commandVariations = new Dictionary<string, Action<string[]>>()
        {
            ["mcc-reach"] = new Action<string[]>((args) => HandleMccReach(args)),
            ["mcc-halo3"] = new Action<string[]>((args) => HandleMccHalo3(args)),
        };

        private static void HandleMccReach(string[] args)
        {
            var (inputUri, outputUri) = ValidatePaths(args);
            IPhysicsModel config = new MCCReachPhysicsModel();
            ICacheContext context = new MCCReachContext();
            PhysicsModel.Generate(config, context, inputUri, outputUri);
        }

        private static void HandleMccHalo3(string[] args)
        {
            var (inputUri, outputUri) = ValidatePaths(args);
            IPhysicsModel config = new MCCHalo3PhysicsModel();
            ICacheContext context = new MCCReachContext();
            PhysicsModel.Generate(config, context, inputUri, outputUri);
        }

        private static Tuple<Uri, Uri> ValidatePaths(string[] args)
        {
            Uri inputUri = (args.Length <= 2) ?
                throw new ArgumentException("Missing input and output URIs.") 
                : InputValidation.ValidatePath(args[2], false);

            Uri outputUri = (args.Length <= 3) ?
                throw new ArgumentException("Missing output URI.")
                : InputValidation.ValidatePath(args[3], true);

            return new Tuple<Uri, Uri>(inputUri, outputUri);
        }

        string RequiredFirstOptions()
        {
            return string.Join("|", commandVariations.Select((item) => item.Key));
        }

        CommandResult ICommandLineOperation.Invoke(string[] args)
        {
            // Only some games supported.
            string gameName = (args.Length <= 1) ?
                throw new ArgumentException("Missing game, input and output URIs.")
                : args[1];

            if (!commandVariations.ContainsKey(gameName))
            {
                return CommandResult.Failed;
            }

            try
            {
                commandVariations[gameName].Invoke(args);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return CommandResult.MoreInformationRequired;
            }
            catch (InvalidOperationException)
            {
                return CommandResult.Failed;
            }
            
            return CommandResult.Success;
        }
    }

    class DetailsCommand : ICommandLineOperation
    {
        public DetailsCommand(List<ICommandLineOperation> commands)
        {
            this.commands = commands;
        }

        private List<ICommandLineOperation> commands;
        public string CommandIdentifier => "more-info";

        string ICommandLineOperation.Usage => CommandIdentifier;

        public string Description => "A tool that stitches other tools together.\n" +
            "The goal is to maintain implementations in their respected tools and " + 
            "over time, benefit from the improvements that are pushed upstream.";

        public CommandResult Invoke(string[] args)
        {
            Console.WriteLine(this.Description);
            foreach (var cmd in commands)
            {
                if (cmd == this)
                {
                    continue;
                }
                Console.WriteLine(cmd.Description);
            }
            return CommandResult.Success;
        }
    }

    static void PrintUsage(List<ICommandLineOperation> commands)
    {
        Console.WriteLine("Usage: one of the following commands, followed by arguments which are " +
            "[optional], <required|OR-other-required>:");
        foreach (var cmd in commands)
        {
            Console.WriteLine(cmd.Usage);
        }
    }

    static void Main(string[] args)
    {
        List<ICommandLineOperation> supportedCommands = new List<ICommandLineOperation> { new PhysicsModelCommand() };
        supportedCommands.Add(new DetailsCommand(supportedCommands));


        if (args.Length >= 1)
        {
            foreach (var cmd in supportedCommands)
            {
                if(cmd.CommandIdentifier.Equals(args[0]))
                {
                    if (cmd.Invoke(args) == CommandResult.Failed)
                    {
                        Console.WriteLine("Command failed.");
                        continue;
                    }
                    
                    return;
                }
            }
        }
        PrintUsage(supportedCommands);
    }

}