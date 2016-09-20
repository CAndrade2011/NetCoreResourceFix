using System;
using System.Collections.Generic;
using Code2C.NetCoreResourceFix.Properties;
using System.IO;
using System.Windows.Forms;


namespace Code2C.NetCoreResourceFix
{
    class Program
    {
        static int Main(string[] args)
        {
            int ret = 0;
            try
            {
                string initialPath = string.Empty;
                if (args != null && args.Length > 0)
                {
                    initialPath = args[0];
                    Console.WriteLine("Procurando arquivos " + Settings.Default.FilterToSearch + " em " + initialPath + ".");
                }

                FixFiles(initialPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                ret = 1;
            }
#if DEBUG
            Console.WriteLine("\nPressione <ENTER> para finalizar.");
            Console.Read();
#endif
            return ret;
        }

        private static void FixFiles(string initialPath = "")
        {
            Console.WriteLine("NetCoreResourceFix V4 e 1/8.");

            Application.DoEvents();
            if (string.IsNullOrEmpty(initialPath))
                initialPath = DefineCaminhoInicial();

            List<string> ret = new List<string>();
            List<string> listFiles = new List<string>();

            if (Directory.Exists(initialPath))
            {
                string[] dirFiles = Directory.GetFiles(initialPath, Settings.Default.FilterToSearch, SearchOption.AllDirectories);
                foreach (string file in dirFiles)
                {
                    FileInfo fi = new FileInfo(file);
                    string name = fi.Name.Replace(fi.Extension, "");
                    if (!name.Contains(".") && !listFiles.Contains(file) && !file.Contains("kendo"))
                        listFiles.Add(file);
                }
            }

            foreach (string file in listFiles)
            {
                FileInfo fi = new FileInfo(file);
                string name = file.Replace(fi.Extension, ".Designer.cs");

                if (File.Exists(name) && !ret.Contains(name))
                {
                    string content = string.Empty;
                    using (StreamReader sr = File.OpenText(name))
                    {
                        content = sr.ReadToEnd();
                        sr.Close();
                    }

                    if (content.Contains(" internal "))
                    {
                        content = content.Replace(" internal ", " public ");
                        File.WriteAllText(name, content);
                        ret.Add(name);
                        Console.WriteLine(file);
                        Application.DoEvents();
                    }
                }

            }

            if (ret != null && ret.Count > 0)
            {
                if (ret.Count > 1)
                    Console.WriteLine("Foram encontrados " + ret.Count.ToString() + " arquivos.");
                else
                    Console.WriteLine("Foi encontrado " + ret.Count.ToString() + " arquivo.");
            }
            else
            {
                Console.WriteLine("Nenhum arquivo encontrado.");
            }
            Application.DoEvents();
        }

        private static string DefineCaminhoInicial()
        {
            string initialPath = Settings.Default.InitialDirectory;
            if (string.IsNullOrEmpty(initialPath))
            {
                FileInfo fi = new FileInfo(Application.ExecutablePath);
                DirectoryInfo di = fi.Directory;
                string path = di.FullName;
                initialPath = di.Root.FullName;
                if (path != initialPath)
                {
                    string pathAux = path;
                    do
                    {
                        path = pathAux;
                        di = di.Parent;
                        pathAux = di.FullName;
                    } while (pathAux != initialPath);
                    initialPath = path;
                }
                Console.WriteLine("Procurando arquivos " + Settings.Default.FilterToSearch + " em caminho automático " + initialPath + ".");
            }
            else
            {
                Console.WriteLine("Procurando arquivos " + Settings.Default.FilterToSearch + " em " + initialPath + ".");
            }
            return initialPath;
        }
    }
}
