using System;
using System.IO;

namespace Toolr.Details
{
    public static class InputValidation
    {
        public static Uri ValidatePath(string pathString, bool isOutput)
        {
            //Assume relative-file at first.
            Uri uri = null;
            try {
                uri = new Uri(pathString);
            } catch (System.UriFormatException)
            {
            }

            if (uri == null)
            {
                string pathString2 = Path.GetFullPath(pathString);
                try
                {
                    uri = new Uri(pathString2);
                }
                catch (System.UriFormatException)
                {
                    throw new ArgumentException(pathString + " is an invalid URI.");
                }
            }

            if (!uri.IsFile)
            {
                //TODO
                throw new ArgumentException("only files are supported URIs for now.");
            }
            else
            {
                if (isOutput)
                {
                    var dir = Path.GetDirectoryName(uri.AbsolutePath);
                    Directory.CreateDirectory(dir); // creates all directories that don't exist in the path.
                }
                else
                {
                    if (!File.Exists(uri.AbsolutePath))
                    {
                        throw new ArgumentException("input path does not exist");
                    }
                }
            }

            return uri;
        }
    }
}
