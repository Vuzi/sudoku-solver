using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ConsoleApplication {
    class Options {

        public static List<String> files { get; protected set; }
        public static bool verbose { get; protected set; }
        public static bool show { get; protected set; }
        public static bool solve { get; protected set; }

        /// <summary>
        /// Show usages
        /// </summary>
        private static void ShowUsage() {
            Console.WriteLine("Usage : " + System.AppDomain.CurrentDomain.FriendlyName + " solve|validate [-hv] sudokuFile [sudokuFile ...]");
        }

        /// <summary>
        /// Show the the help (-h or --help)
        /// </summary>
        private static void ShowHelp() {
            ShowUsage();
            Console.WriteLine(String.Format("{0,-15} : Print this help", "-h|--help"));
            Console.WriteLine(String.Format("{0,-15} : Print more informations", "-v|--verbose"));
            Console.WriteLine(String.Format("{0,-15} : Show the sudoku", "-s|--show"));

            var version = Assembly.GetEntryAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
            TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
            TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)

            #if DEBUG
            Console.WriteLine("Compiled (debug) : " + buildDateTime + " v" + version + " by Vuzi");
            #else
            Console.WriteLine("Compiled : " + buildDateTime + " v" + version + " by Vuzi");
            #endif
        }

        /// <summary>
        /// Parse the given args
        /// </summary>
        /// <param name="args">The args to parse, provided to the entry point of the application</param>
        public static void ParseArgs(string[] args) {

            // Init static values
            files = new List<string>(); ;
            verbose = false;

            // Test args lenght
            if (args.Length < 1) {
                Console.WriteLine("Error : no enough arguments");
                ShowUsage();
                System.Environment.Exit(1);
            }

            // First, the mode
            switch (args[0]) {
                case "solve" :
                    solve = true;
                    break;
                case "validate":
                    solve = false;
                    break;
                case "-h":
                case "--help":
                    ShowHelp();
                    System.Environment.Exit(0);
                    break;
                default: // Error
                    Console.WriteLine("Error : unknown mode '" + args[0] + "'");
                    ShowUsage();
                    System.Environment.Exit(1);
                    break;
            }

            // Handle args
            for (int i = 1; i < args.Length; i++) {
                String arg = args[i];

                if (arg.StartsWith("-")) {
                    if (arg.StartsWith("--")) {
                        // Long argument
                        switch (arg.Substring(2)) {
                            case "help": // Help
                                ShowHelp();
                                System.Environment.Exit(0);
                                break;
                            case "show": // Help
                                show = true;
                                break;
                            case "verbose": // Verbose
                                verbose = true;
                                break;
                            default: // Error
                                Console.WriteLine("Error : unknown argument '" + arg + "'");
                                ShowUsage();
                                System.Environment.Exit(1);
                                break;
                        }
                    } else {
                        // Short
                        foreach (char c in arg.Substring(1)) {
                            switch (c) {
                                case 'h': // Help
                                    ShowHelp();
                                    System.Environment.Exit(0);
                                    break;
                                case 's': // Show
                                    show = true;
                                    break;
                                case 'v': // Verbose
                                    verbose = true;
                                    break;
                                default: // Error
                                    Console.WriteLine("Error : unknown argument '" + c + "'");
                                    ShowUsage();
                                    System.Environment.Exit(1);
                                    break;
                            }
                        }
                    }
                } else {
                    files.Add(arg);
                }
            }

            if (files.Count <= 0) {
                Console.WriteLine("Error : no files provided");
                ShowUsage();
                System.Environment.Exit(1);
            }
        }

    }
}
