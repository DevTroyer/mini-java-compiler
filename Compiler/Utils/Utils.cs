using System.IO;

namespace Compiler {
    public class Utils : Resources {
        /// <summary>
        /// Displays a single token along with attributes of the token.
        /// </summary>
        public void DisplayToken() {
            Console.WriteLine("{0,-8}{1,-12}{2,-40}{3,-14}{4,-14}{5,-40}", lineNumber, token, lexeme, value, valueR, literal);
        }
    }
}