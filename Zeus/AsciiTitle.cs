﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus
{
    public static class AsciiTitle
    {
        public const string Title =
            @" _____               
/ _  / ___ _   _ ___ 
\// / / _ \ | | / __|
 / //\  __/ |_| \__ \
/____/\___|\__,_|___/";

        public static void WriteTitle()
        {
            Console.WriteLine();
            Console.WriteLine(Title);
        }
    }
}
