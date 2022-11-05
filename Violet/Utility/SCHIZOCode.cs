using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Violet.Graphics;

namespace Violet.Utility
{
    /// <summary>
    /// HEAR YE HEAR YE!
    /// THIS CODE IS BAD!
    /// IT *REALLY* SHOULDN'T BE USED!
    /// BUT IT IS ANYWAYS!
    /// </summary>
    public static class SCHIZOCode
    {

        public static Graphic SCHIZO_GraphicFromBase64(byte[] b64, int depth) {

            Image FCT = new Image(b64);
            
            //create graphic
            Graphic graph = new Graphic(
                //b64 byte array
                b64, 
                // position
                new Vector2f(0, 0),
                
                // i don't know man
                new IntRect(new Vector2i(0, 0), 
                new Vector2i(
                    (int)FCT.Size.X, 
                    (int)FCT.Size.Y)
                ), 
                new Vector2f(0, 0),
                depth);
            return graph;
        }
    }
}
