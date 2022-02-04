using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Configuration;

namespace ShedLab {
    public class Logger {
        public enum LogLevel {
            Verbose,
            Info,
            Warning,
            Error,
            None
        }

        public static LogLevel logLevel = LogLevel.None;

        const int MAX_BUFFER_SIZE = 1000;
        const int FLUSH_EVERY_SECONDS = 10;

        static StringBuilder logBuffer = new StringBuilder();
        static DateTime logLastFlushedTime = DateTime.Now;

        public static void Log(Exception ex) {
            try {
                Write($"{DateTime.Now.ToString()}\t{LogLevel.Error.ToString()}\t{ex.Message}");
                Write("Stack Trace");
                Write("-----------");
                Write(ex.StackTrace);
            } catch (Exception ex2) {
                Terminate(MethodBase.GetCurrentMethod(), ex2);
            }
        }

        public static void Log(LogLevel LogLevel, string Message) {
            try {
                if ((int)logLevel <= (int)LogLevel)
                    Write($"{DateTime.Now.ToString()}\t{LogLevel.ToString()  }\t{Message}");
            } catch (Exception ex) {
                Terminate(MethodBase.GetCurrentMethod(), ex);
            }
        }

        // If we have exceptions for any of the Log methods, then 
        // best thing is to terminate, and display exception on 
        // console so that problem can be manually fixed.

        // Write to Log Buffer
        private static void Write(string Text) {
            Console.WriteLine(Text);

            try {
                logBuffer.Append("\r\n" + Text);
                if (logBuffer.Length > MAX_BUFFER_SIZE)
                    Flush();
                if ((DateTime.Now - logLastFlushedTime).TotalSeconds >= FLUSH_EVERY_SECONDS) {
                    Flush();
                }

            } catch (Exception ex) {
                Terminate(MethodBase.GetCurrentMethod(), ex);
            }
        }

        // Flush Log Buffer to File
        public static void Flush() {
            try {
                WriteToFile();
                logBuffer.Clear();
                logLastFlushedTime = DateTime.Now;
            } catch (Exception ex) {
                Terminate(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private static void WriteToFile() {
            try {
                CreateLogFolderIfNotExist();
                string combinedPathAndFile = Path.Combine(GetLogPath(), GetFileName());
                File.AppendAllText(combinedPathAndFile, logBuffer.ToString());
            } catch (Exception ex) {
                Terminate(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private static void CreateLogFolderIfNotExist() {
            try {
                Directory.CreateDirectory(GetLogPath());
            } catch (Exception ex) {
                Terminate(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private static string GetFileName() {
            try {
                return $"Log-{DateTime.Now.ToString("yyyy-MM-dd")}.log";
            } catch (Exception ex) {
                Terminate(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }

        private static string GetLogPath() {
            try {
                return (Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs"));
            } catch (Exception ex) {
                Terminate(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }



        // Terminate elegantly with exception. If we can't log, then the program can't run as we wont be able to see that the process is working.
        static void Terminate(MethodBase CurrentMethod, Exception ex) {
            Console.WriteLine($"Exception in {CurrentMethod.ReflectedType.Name}.{CurrentMethod.Name}()");
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Terminating: {Assembly.GetExecutingAssembly().GetName().Name}");
            Console.WriteLine($" ");
            Console.WriteLine($"Stack Trace");
            Console.WriteLine($"-----------");
            Console.WriteLine($"{ex.StackTrace.Trim()}");

            Environment.Exit(1);
        }
    }
}

