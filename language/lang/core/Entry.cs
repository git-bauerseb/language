using System;
using System.Collections.Generic;
using System.IO;
using lang.core;

namespace language.lang.core {
    public class Entry {

        static bool hadError = false;
        private static bool hadRuntimeError = false;

        private static readonly Interpreter _interpreter = new Interpreter();
        
        public static void error(int line, string msg) {
            report(line, "", msg);
        }

        public static void error(Token token, String msg) {
            if (token.type == TokenType.EOF) {
                report(token.line, " at end", msg);
            }
            else {
                report(token.line, " at '" + token.Lexeme + "'", msg);
            }
        }

        public static void runtimeError(RuntimeError error) {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine(error.Message + "\n[line " + error.Token.line + "]");
            hadRuntimeError = true;
        }

        private static void report(int line, String where, String msg) {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine("line " + line + ", Error" + where + ": " + msg);
        }

        private static void run(string source) {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            List<Stmt> expression = parser.parse();

            if (hadError || hadRuntimeError) {
                return;
            }

            _interpreter.interpret(expression);
        }

        private static void runFile(String path) {
            byte[] bytes = File.ReadAllBytes(path);
            run(System.Text.Encoding.UTF8.GetString(bytes));

            if (hadError) {
                return;
                //Environment.Exit(1);
            }
        }

        private static void runPrompt() {
            for (;;) {
                Console.Write(">>> ");
                string input = Console.ReadLine();

                if (input == null) {
                    break;
                }

                run(input);
                hadError = false;
            }
        }
        

        public static void Main(string[] args) {
            
            if (args.Length > 1) {
                Console.WriteLine("Usage: [script]");
            } else if (args.Length == 1) {
                runFile(args[0]);
            } else {
                runPrompt();
            }
        }
    }
}