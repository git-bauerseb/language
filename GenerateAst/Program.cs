using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateAst {
    
    class Program {

        private static void defineType(TextWriter writer, string baseName, string className, string fieldList) {
            
            // Class definition
            writer.WriteLine(" public class " + className + " : " + baseName + "{");
            
            // Constructor
            writer.WriteLine("  " + className + "(" + fieldList + ") {");

            string[] fields = fieldList.Split(", ");
            foreach (string s in fields) {
                string name = s.Split(" ")[1];
                writer.WriteLine("  this." + name + " = " + name + ";");
            }
            
            writer.WriteLine("  }\n");
            foreach (var field in fields) {
                writer.WriteLine("  public readonly " + field + ";");
            }
            
            
            writer.WriteLine();
            writer.WriteLine("  public override R  accept<R>(Visitor<R> visitor) {");
            writer.WriteLine("      return visitor.visit" + className + baseName + "(this);");
            writer.WriteLine("  }");
            
            
            // Class end
            writer.WriteLine("  }");
        }

        private static void defineVisitor(TextWriter writer, string baseName, List<String> types) {
            
            writer.WriteLine("  public interface Visitor<R> {");

            foreach (var type in types) {
                string typeName = type.Split(":")[0].Trim();
                writer.WriteLine("  R visit" + typeName + baseName + "(" +
                                 baseName + "." + typeName + " " + baseName.ToLower() + ");");
            }
            
            writer.WriteLine("  }");
        }
        
        private static void defineAst(string outputDir, string baseName, List<string> types) {
            string path = outputDir + "/" + baseName + ".cs";
            TextWriter writer = new StreamWriter(new FileStream(path, FileMode.Create),
                Encoding.UTF8);
            writer.WriteLine("using System.Collections.Generic;\nusing lang.core;");
            writer.WriteLine("\nnamespace lang.core {");

            defineVisitor(writer, baseName, types);
            
            writer.WriteLine("public abstract class " + baseName + " {");
            
            writer.WriteLine();
            writer.WriteLine("  public abstract R accept<R>(Visitor<R> visitor);");

            foreach (var type in types) {
                string className = type.Split(":")[0].TrimEnd();
                string fields = type.Split(":")[1].Trim();
                defineType(writer, baseName, className, fields);
            }
            
            writer.WriteLine("}\n}");
            writer.Close();
        }
        
        static void Main(string[] args) {
            if (args.Length != 1) {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("Usage: generate_ast <output_dir>");
            }

            string outputDir = args[0];
            defineAst(outputDir, "Expr", new string[] {
                "Binary : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal : object value",
                "Unary : Token op, Expr right",
                "Variable : Token name"
            }.ToList());
            
            defineAst(outputDir, "Stmt", new string[] {
                "Expression : Expr expression",
                "Print : Expr expression"
            }.ToList());
        }
    }
}