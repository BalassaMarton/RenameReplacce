using System;
using System.IO;
using System.Linq;

namespace RenameReplace
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 3)
                    throw new InvalidOperationException(
                        "Usage: RenameReplace <working dir> <old text> <new text>");
                var context = new RenameReplaceContext();
                context.WorkingDir = args[0];
                context.OldText = args[1];
                context.NewText = args[2];
                Execute(context);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        static void Execute(RenameReplaceContext context)
        {
            FindFiles(context);
            ScanFiles(context);
            RenameFiles(context);
        }

        static void FindFiles(RenameReplaceContext context)
        {
            FindFiles(context.WorkingDir, context);
        }

        static void FindFiles(string dir, RenameReplaceContext context)
        {
            foreach (var fileName in Directory.GetFiles(dir)
                .Where(f => (File.GetAttributes(f) & (FileAttributes.Hidden | FileAttributes.Directory)) == 0))
            {
                context.FilesToScan.Add(fileName);
                if (Path.GetFileName(fileName).ToLower().Contains(context.OldText.ToLower()))
                    context.FilesToRename.Add(fileName);
            }

            foreach (var dirName in Directory.GetDirectories(dir)
                .Where(f => (File.GetAttributes(f) & FileAttributes.Hidden) == 0))
            {
                if (Path.GetFileName(dirName).ToLower().Contains(context.OldText.ToLower()))
                    context.FilesToRename.Add(dirName);
                FindFiles(dirName, context);
            }
        }

        static void ScanFiles(RenameReplaceContext context)
        {
            foreach (var fileName in context.FilesToScan)
                ScanFile(fileName, context);
        }

        static void ScanFile(string fileName, RenameReplaceContext context)
        {
            string oldText;
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                    oldText = reader.ReadToEnd();
            }

            var text = oldText.Replace(context.OldText, context.NewText);
            if (text != oldText)
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream))
                        writer.Write(text);
                }

                Console.WriteLine($"Mofified {Path.GetRelativePath(context.WorkingDir, fileName)}");
            }
        }

        static void RenameFiles(RenameReplaceContext context)
        {
            foreach (var fileName in Enumerable.Reverse(context.FilesToRename))

            {
                var oldName = Path.GetFileName(fileName);
                var newName = oldName.Replace(context.OldText, context.NewText);
                if (!string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                {
                    newName = Path.Combine(Path.GetDirectoryName(fileName), newName);
                    Directory.Move(fileName, newName);
                    Console.WriteLine(
                        $"Renamed {Path.GetRelativePath(context.WorkingDir, fileName)} to {Path.GetRelativePath(context.WorkingDir, newName)}");
                }
            }
        }
    }
}