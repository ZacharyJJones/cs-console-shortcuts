using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ConsoleUtils
{
    /// <summary> Shortcut class to <see cref="Utils"/> </summary>
    // ReSharper disable once UnusedType.Global -- Intended for public use.
    // ReSharper disable once InconsistentNaming -- Lowercase 'c' used for convenience.
    public class c : Utils { }

    //public partial class ExampleClass
    //{
    //    public void ExampleMethod()
    //    {
    //        var data = new List<string> { };
    //        ProgramLoop(null, data, new List<(string name, Action method)>
    //            {
    //                ("Option A", () => { int a = 1; a += 12; Console.WriteLine(a); }),
    //                ("Option B", () => { return; /* Do literally anything you want, that returns void. */ }),
    //                ("Option C", () => ProgramFunction("Option Sub-C", data, () => { /* body of function */ }, "hello", 2, '\t'))
    //            }
    //        );
    //    }
    //}

    // Console Feature Shortcuts
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Class members will have inconsistent names so they can act as shortcuts.")]
    public partial class Utils
    {
        /// <summary> Shortcut to <see cref="Console.WindowWidth"/>. </summary>
        public static int Width
        {
            get => Console.WindowWidth;
            set => Console.WindowWidth = value;
        }

        /// <summary> Shortcut to <see cref="Console.WindowHeight"/>. </summary>
        public static int Height
        {
            get => Console.WindowHeight;
            set => Console.WindowHeight = value;
        }

        /// <summary> Shortcut to <see cref="Console.WriteLine()"/>. </summary>
        public static void w() => Console.WriteLine();

        /// <summary> Shortcut to <see cref="Console.WriteLine(string)"/>. </summary>
        /// <param name="input"> The <see cref="string"/> to write to the console, followed by the current line terminator. </param>
        public static void w(string input) => Console.WriteLine(input);

        /// <summary> Shortcut to <see cref="Console.Write(string)"/>. </summary>
        /// <param name="input"> The <see cref="string"/> to write to the console. </param>
        public static void w_(string input) => Console.Write(input);

        /// <summary> Shortcut to <see cref="ww(IEnumerable{string})"/>, allowing for a params-style input. </summary>
        /// <param name="input"> The <see cref="string"/> collection to write to the console. </param>
        public static void ww(params string[] input) => ww(input.AsEnumerable());

        /// <summary> Calls <see cref="Console.WriteLine(string)"/> for each <see cref="string"/> value in the argument [input]. </summary>
        /// <param name="input"> The <see cref="string"/> collection to write to the console. </param>
        public static void ww(IEnumerable<string> input)
        {
            foreach (string s in input)
            {
                Console.WriteLine(s);
            }
        }

        /// <summary> Calls <see cref="Console.WriteLine(string)"/> using [<paramref name="input"/>], then returns the value of <see cref="Console.ReadLine()"/> </summary>
        /// <param name="input"> The <see cref="string"/> which will be written to the console. </param>
        /// <param name="sayPressEnter"> A <see cref="bool"/> value which describes if the user will be told to press enter after displaying [<paramref name="input"/>]. </param>
        public static string wr(string input, bool sayPressEnter = true)
        {
            Console.WriteLine($"{input}{(sayPressEnter ? " Press [Enter] to continue." : "")}");
            return Console.ReadLine();
        }


        /// <summary> Shortcut to <see cref="Console.Clear()"/>. </summary>
        public static void c() => Console.Clear();

        /// <summary> Shortcut to <see cref="Console.ReadLine()"/>. </summary>
        public static string r() => Console.ReadLine();

        /// <summary> Shortcut to <see cref="Console.Read()"/>. </summary>
        public static int r_() => Console.Read();

        /// <summary> "ri" meaning "Read Int", this method asks for user input and attempts to parse it into an integer. </summary>
        public static bool ri(out int value) => int.TryParse(Console.ReadLine(), out value);

        /// <summary> 
        /// "ri" meaning "Read Int", this method asks for user input and attempts to parse it into an integer. 
        /// <para />
        /// This version of the method will not return until a valid value is given.
        /// </summary>
        public static int ri()
        {
            int ret;
            while (!ri(out ret))
            {
                Console.WriteLine("> Invalid integer value given. Try again.");
            };
            return ret;
        }

        /// <summary> Shortcut to <see cref="Console.ReadKey()"/>. </summary>
        public static ConsoleKeyInfo rk() => Console.ReadKey();
    }




    // Actual Utilities
    public partial class Utils
    {
        /// <summary> A preset which points to "ProgramLoopCustom" which incorporates a "Pick From List" loop which is very common in console applications. </summary>
        /// <param name="loopName"> The 'Name' of this section, which will be added to [<paramref name="navStack"/>]. If set as null, it will not be added to the NavList. </param>
        /// <param name="navStack"> The navigation stack which is being used for this console application. </param>
        /// <param name="prePickDisplay"> The code to execute immediately after displaying the nav stack but before choosing an option. </param>
        /// <param name="pickOptions"> A set of options to be used by the "Pick" method as a part of this program loop. </param>
        /// <param name="loopExitMessage"> The message to display with the PickOption which will cause the loop to exit. </param>
        public static void ProgramLoop(string loopName, List<string> navStack, Action prePickDisplay, IEnumerable<(string name, Action method)> pickOptions, string loopExitMessage = "Exit This Section")
        {
            ProgramLoop<Action>(loopName, navStack, prePickDisplay, pickOptions, (act) => act.Invoke(), loopExitMessage);
        }

        public static void ProgramLoop<T>(string loopName, List<string> navStack, Action prePickDisplay, IEnumerable<(string, T)> pickOptions, Action<T> postPickExecuteWithChosen, string loopExitMessage = "Exit This Section")
        {
            // using a wrapper as a workaround to edit the 'determineIfRepeating' method later on.
            // -- I dislike this workaround, but no better solution was found.
            // -- this was the only way found to 'inject' the 'leave this section' option into the pick options list.
            // ==>> Could replace this "wrapper" option with an actual class which contains the func that determines normally if continuing, but has...
            //   ... an option which when set will cause it to always return false, allowing this bypass to occur in a more specific way!
            //   ... In a sense, it is not any different (still a wrapper), however, you don't have the weirdness of editing the wrapper value itself.
            var determineIfRepeat = Wrapper.FuncReturnTrue;

            // set up actual action which gets called.
            void LoopBody()
            {
                DrawNavStack(navStack);
                prePickDisplay?.Invoke();

                // ReSharper disable once PossibleMultipleEnumeration
                // -- copy list each time, so it can be updated outside of this method.
                var optionsCopy = pickOptions.ToList();
                optionsCopy.Insert(0, (loopExitMessage, default));

                // get the chosen option
                var chosen = Pick(optionsCopy, (x) => x.Item1, true);

                // check if chosen option was "cancel". if so, set the determine if repeat value to false so it may be exited
                if (chosen.Equals(optionsCopy[0]))
                {
                    determineIfRepeat.Value = Wrapper.FuncReturnFalse.Value;
                }
                else
                {
                    postPickExecuteWithChosen.Invoke(chosen.Item2);
                }
            }

            ProgramLoopCustom(loopName, navStack, LoopBody, determineIfRepeat);
        }

        // allows for potentially more specific code execution while retaining the program loop structure.
        /// <summary> A method which acts as an abstraction of a loop which checks if you want to exit the loop after each iteration, which is a very common console application programming pattern. </summary>
        /// <param name="programLoopName"> The 'Name' of this program loop, which will be added to [<paramref name="navStack"/>] and displayed alongside the other items in the NavList. </param>
        /// <param name="navStack"> The navigation stack which is being used for this console application. </param>
        /// <param name="loopBody"> The method to execute while in this program loop. </param>
        /// <param name="determineIfRepeating"> The function which determines if the method will repeat. Uses a wrapper so that the program loop method can alter the return result correctly. </param>
        public static void ProgramLoopCustom(string programLoopName, List<string> navStack, Action loopBody, Wrapper<Func<bool>> determineIfRepeating = null)
        {
            // null checks
            determineIfRepeating = determineIfRepeating ?? new Wrapper<Func<bool>>(() =>
            {
                WriteDivider();
                Console.WriteLine($"Enter 'x' to exit this \"{programLoopName ?? "?Unknown?"}\" section.");
                return !CheckForInput("x");
            });

            // start
            if (programLoopName != null)
            {
                navStack?.Add(programLoopName);
            }

            // loop
            do
            {
                loopBody?.Invoke();
            }
            while (determineIfRepeating.Value.Invoke());

            // cleanup
            if (programLoopName != null)
            {
                navStack?.RemoveAt(navStack.Count - 1);
            }
        }

        /// <summary> Clears console and writes the NavList to the console, showing the user where they are at overall. </summary>
        /// <param name="navStack"> The hierarchy guide which is being used in this console application. </param>
        public static void DrawNavStack(List<string> navStack)
        {
            // Setup
            Console.Clear();
            WriteDivider(false);
            Console.WriteLine();

            // Can't display if null
            if (navStack == null)
            {
                return;
            }

            for (int i = 0; i < navStack.Count; i++)
            {
                string itemPrepend = ((i == 0) ? "" : "|> ").PadLeft(i);
                Console.WriteLine($"{itemPrepend}{navStack[i]}");
            }
            WriteDivider();
        }


        /// <summary> A 'Pick' overload which accepts a simpler List input and a function to get a <see cref="string"/> value for each <typeparamref name="T"/> of [<paramref name="options"/>]. </summary>
        /// <typeparam name="T"> The <see cref="Type"/> of the [<paramref name="options"/>] List being given. </typeparam>
        /// <param name="options"> The set of objects which will be chosen from. </param>
        /// <param name="nameFromOptionFunc"> Can be null. A function which describes how to acquire a <see cref="string"/> for each <typeparamref name="T"/> of [<paramref name="options"/>]. </param>
        /// <param name="displayZeroBased"> If true, the first option will display with selection number '0' with a line separating it from other options. </param>
        public static T Pick<T>(IEnumerable<T> options, Func<T, string> nameFromOptionFunc = null, bool displayZeroBased = false)
        {
            nameFromOptionFunc = nameFromOptionFunc ?? ((generic) => generic.ToString());

            var optionsWithNames = options?.Select(option => (nameFromOptionFunc.Invoke(option), option));

            return Pick(optionsWithNames, displayZeroBased);
        }

            
        /// <summary> Generic method for choosing an option with a name from a list. </summary>
        /// <typeparam name="T"> The <see cref="Type"/> of the object to be returned. </typeparam>
        /// <param name="options"> The set of options to choose a result from.  </param>
        /// <param name="displayZeroBased"> If true, the first option will display with selection number '0' with a line separating it from other options. </param>
        public static T Pick<T>(IEnumerable<(string name, T obj)> options, bool displayZeroBased = false/*, int columns = 1*/)
        {
            // "Const" -level variables
            var optionsList = options.ToList();
            const string NUMBER_DISPLAY = "[{1}]";
            
            // Setup
            int maxNumPadding = optionsList.Count.ToString().Length + NUMBER_DISPLAY.Length - 3;
            string numberDisplayFormat = NUMBER_DISPLAY.PadLeft(maxNumPadding);
            
            // Initial header, and catching nulls / empty lists
            Console.WriteLine("> Enter a number to choose an option.");
            if (optionsList.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("> No Pick Options Available.");
                Console.WriteLine("> Press 'Enter' to continue.");
                Console.ReadLine();
                return default;
            }
            
            // @TODO -- send this to itemsList instead of outputting directly to the console?
            
            // Setup & Display Options
            int itemNum = displayZeroBased ? 0 : 1;
            if (displayZeroBased) optionsList[0] = (optionsList[0].name + "\n", optionsList[0].obj);
            foreach (var option in optionsList)
            {
                Console.WriteLine($"{string.Format(numberDisplayFormat, itemNum)} - {option.name}");
                itemNum++;
            }
            
            // Get (Valid) User Choice
            int retIndex;
            while (!_pick_parseHelper(Console.ReadLine(), optionsList.Count, displayZeroBased, out retIndex))
            {
                Console.WriteLine("> Bad input, try again.");
            }

            // Return Value Chosen
            return optionsList[retIndex - (displayZeroBased ? 0 : 1)].obj;
        }
        private static bool _pick_parseHelper(string input, int listCount, bool displayZeroBased, out int value)
        {
            // Check If Parse-able
            if (!int.TryParse(input, out value))
                return false;

            // Set Up Validity Constraints
            int validLow = 1 - ((displayZeroBased) ? 1 : 0);
            int validHigh = listCount - ((displayZeroBased) ? 1 : 0);
            
            // Determine validity
            return (validLow <= value && value <= validHigh);
        }

        /// <summary> Reads the console for user input and returns a <see cref="bool"/> indicating whether that value was an exact match with [<paramref name="inputNeeded"/>]. </summary>
        /// <param name="inputNeeded"> The <see cref="string"/> value which must be entered by the user in order for this method to return [true]. </param>
        /// <param name="explanation"> An explanation to be written to the console before asking the user to enter a certain input. Leave as [null] to display nothing. </param>
        public static bool CheckForInput(string inputNeeded, string explanation = null)
        {
            if (explanation != null) 
                Console.WriteLine(explanation);

            var input = Console.ReadLine();
            var inputIsValid = (input == inputNeeded);

            Console.WriteLine($"> Input is{(inputIsValid ? " " : " Not ")}Valid.");
            return inputIsValid;
        }

        /// <summary> "Aligns" each item in a column by padding the shorter strings with spaces until they are the same length as the longest. </summary>
        /// <param name="items"> The items to align to each other within a given column. </param>
        /// <param name="columns">  </param>
        /// <param name="lengths">  </param>
        public static IEnumerable<string> AlignItems(IEnumerable<string> items, int columns, out int[] lengths)
        {
            var itemsList = items.ToList();
            lengths = _getColumnLengths(itemsList, columns);

            var lens = lengths.ToArray();
            return itemsList.Select((x, i) => x.PadRight(lens[i % columns]));
        }
        
        // @TODO add other method like ItemsList but displays in leftmost column then the next. etc
        // -- could maybe abstract into pair of "left to right" and "up then down" values?
        
        /// <summary> Returns given [<paramref name="inputItems"/>] in a table format. This overload has all options exposed. </summary>
        /// <param name="columns">The number of columns to display per line. </param>
        /// <param name="cornerChar">The character to display at the row/column intersections of the returned table. </param>
        /// <param name="rowChar">The character to display as the top and bottom edges of the returned table, as well as separating the rows of the table. </param>
        /// <param name="colChar">The character to display as the left and right edges of the returned table, as well as separating the columns of the table. </param>
        /// <param name="inputItems">The items which you want to display as a table. </param>
        public static IEnumerable<string> ItemsList(IEnumerable<string> inputItems, int columns, char cornerChar, char rowChar, char colChar)
        {
            // Enumerate to array.
            var inputArray = inputItems as string[] ?? inputItems.ToArray();
            
            // Setup
            columns = Math.Max(1, columns);
            var aligned = AlignItems(inputArray, columns, out var lengths).ToList();
            var ret = new List<string>();
            
            // getting middle content lines
            int index = 0;
            while ((index + (columns - 1)) < aligned.Count)
            {
                // grab stuff in groups of 'x', where 'x' is the number of columns
                var line = aligned.GetRange(index, columns);
                ret.Add($"{colChar} {string.Join($" {colChar} ", line)} {colChar}");
                index += columns;
            }

            // Fill out the trailing line, if any (when: inputItems.Length % columns != 0)
            if (index - aligned.Count != 0)
            {
                // 10 total (last index is 9)
                // 0, 3, 6, 9
                //  .2 .5 .8 x11

                // 10 total
                // index was 8
                // columns == 3
                
                // [8] == item
                // [9] is out of range
                // [10] is out of range
                
                // if not an exact match, need to do last few on own.
                var line = aligned.GetRange(index, index - aligned.Count);
                for (int i = line.Count; i < columns; i++)
                {
                    // get a padded string with length equal to longest length in this column
                    line.Add("".PadRight(lengths[i]));
                }
                ret.Add($"{colChar} {string.Join($" {colChar} ", line)} {colChar}");
            }

            string tableHeader = $"{cornerChar}{"".PadRight(ret[0].Length - 2, rowChar)}{cornerChar}";
            ret.Insert(0, tableHeader);
            ret.Add(tableHeader);
            return ret;
        }

        private static int[] _getColumnLengths(IEnumerable<string> input, int columns)
        {
            var inputItems = input as string[] ?? input.ToArray();
            
            var lengths = new int[columns];
            for (int i = 0; i < inputItems.Length; i++)
            {
                int column = i % columns;
                lengths[column] = Math.Max(lengths[column], inputItems[i].Length);
            }

            return lengths;
        }


        /// <summary> Displays a 'Pick' menu based on the enum type given. </summary>
        /// <typeparam name="TEnum"> The <see cref="Type"/> of the enum to be picked from. </typeparam>
        /// <param name="valuesToIgnore"> A list of enum values to be ignored for this 'Pick' menu. </param>
        public static TEnum PickEnum<TEnum>(params TEnum[] valuesToIgnore) where TEnum : struct
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            return Pick(values.Except(valuesToIgnore));
        }


        /// <summary> Writes a line to the console using [dividerChar] across the entire width of the <see cref="Console"/> window, with an empty line above and below the divider. </summary>
        /// <param name="includeSpacerLines"> A <see cref="bool"/> value which allows the method to skip writing the empty spacer lines to the console. </param>
        /// <param name="dividerChar"> The <see cref="char"/> to use when writing the divider line to the console. </param>
        public static void WriteDivider(bool includeSpacerLines = true, char dividerChar = '=')
        {
            if (includeSpacerLines) Console.WriteLine();
            Console.WriteLine("".PadRight(Console.WindowWidth - 1, dividerChar));
            if (includeSpacerLines) Console.WriteLine();
        }


        public static string PadL(string input, int maxLength, char padChar = ' ') => input.PadLeft(maxLength, padChar);
        public static string PadR(string input, int maxLength, char padChar = ' ') => input.PadRight(maxLength, padChar);
        
        
        
        
        public static void SetForeColor(ConsoleColor color)
        {
            // Check before setting, as it takes much longer to set than to check.
            if (Console.ForegroundColor != color)
            {
                Console.ForegroundColor = color;
            }
        }

        public static void WriteColor(IEnumerable<IEnumerable<(string Text, ConsoleColor Color)>> colorLines)
        {
            var startFore = Console.ForegroundColor;

            foreach (var lineInfo in colorLines)
            {
                foreach (var (text, color) in lineInfo)
                {
                    SetForeColor(color);
                    Console.Write(text);
                }
                Console.WriteLine();
            }

            SetForeColor(startFore);
        }
    }


    public partial class Utils
    {
        /// <summary> A static non-generic class which provides convenient properties for acquiring certain <see cref="Wrapper{T}"/> presets. </summary>
        public static class Wrapper
        {
            /// <summary> Returns a new <see cref="Wrapper"/>&lt;Func&lt;<see cref="bool"/>&gt;&gt; <see cref="object"/> whose encapsulated function always returns [true]. </summary>
            public static Wrapper<Func<bool>> FuncReturnTrue => new Wrapper<Func<bool>>(() => true);

            /// <summary> Returns a new <see cref="Wrapper"/>&lt;Func&lt;<see cref="bool"/>&gt;&gt; <see cref="object"/> whose encapsulated function always returns [false]. </summary>
            public static Wrapper<Func<bool>> FuncReturnFalse => new Wrapper<Func<bool>>(() => false);
        }

        /// <summary> A class which simply contains a single field of the given <see cref="Type"/>. </summary>
        /// <typeparam name="T"> The <see cref="Type"/> of the field which this class encapsulates. </typeparam>
        public class Wrapper<T>
        {
            /// <summary> Creates a new instance of the <see cref="Wrapper{T}"/> class with the "Value" field being assigned the default value. </summary>
            public Wrapper()
                : this(default)
            { }

            /// <summary> Creates a new instance of the <see cref="Wrapper{T}"/> class with the "Value" field being assigned the given [<paramref name="value"/>]. </summary>
            /// <param name="value"> The <typeparamref name="T"/> value to assign to this <see cref="Wrapper{T}"/> instance's "Value" field. </param>
            public Wrapper(T value)
            {
                Value = value;
            }

            /// <summary> The value which this <see cref="Wrapper{T}"/> class encapsulates. </summary>
            public T Value { get; set; }
        }
    }


    // "Console art" section
    public partial class Utils
    {
        public static class Draw
        {
            public static string Bounce(int i, int length)
            {
                // examples in this method assume length equals 20.
                int halfModulo = length;
                int phaseModulo = halfModulo * 2;

                int phase = (i % phaseModulo) - halfModulo;
                // -20 -19 -18 -17 -16 -15 -14 -13 -12 -11
                // -10  -9  -8  -7  -6  -5  -4  -3  -2  -1   
                //   0   1   2   3   4   5   6   7   8   9
                //  10  11  12  13  14  15  16  17  18  19 

                if (phase < 0) phase++;
                // -19 -18 -17 -16 -15 -14 -13 -12 -11 -10
                //  -9  -8  -7  -6  -5  -4  -3  -2  -1   0
                //   0   1   2   3   4   5   6   7   8   9
                //  10  11  12  13  14  15  16  17  18  19

                phase = Math.Abs(phase);
                //  19  18  17  16  15  14  13  12  11  10
                //   9   8   7   6   5   4   3   2   1   0
                //   0   1   2   3   4   5   6   7   8   9
                //  10  11  12  13  14  15  16  17  18  19

                // when phase == 0, should be at far right
                // when phase == (halfModulo - 1), should be at far left
                // index of '#' within a string of length 'halfModulo' == [halfModulo.Length - 1 - phase]
                int charArrayIndex = halfModulo - 1 - phase;
                //   0   1   2   3   4   5   6   7   8   9
                //  10  11  12  13  14  15  16  17  18  19
                //  19  18  17  16  15  14  13  12  11  10
                //   9   8   7   6   5   4   3   2   1   0

                var charArray = "".PadRight(halfModulo, ' ').ToArray();
                charArray[charArrayIndex] = '#';

                return $"<{new string(charArray)}>";
            }
        }
    }
}
