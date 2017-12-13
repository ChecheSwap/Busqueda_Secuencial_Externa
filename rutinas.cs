using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace searches.ROUTINES
{
    public class rutinas
    {
        public observer secuencialiterativa(ref string[] source, string key, posicionArchivo commit)
        {
            return this.iterative(source, key, commit);
        }

        public observer secuencialrecursive(ref string [] source, string key, posicionArchivo commit)
        {
            var send = new observer();

            try
            {
                return this.recursive(ref source, key, 0, ref send, commit);
            }
            catch(Exception ex)
            {
                msg.error(ex.ToString());
            }

            return send;
        }        

        private observer recursive(ref string [] source, string key, int pos, ref observer result, posicionArchivo commit)
        {

                if (pos == source.Length)
                {
                    return result;
                }
                else
                {

                    if (source[pos].Trim().Equals(key))
                    {
                        if (!result.flag)
                        {
                            result.flag = true;

                            if (commit == posicionArchivo.FIRST)
                            {
                                result.cantidad++;
                                return result;
                            }
                        }

                        result.cantidad++;
                    }

                    recursive(ref source, key, ++pos, ref result, commit);
                }

            return result;
        }

        private observer iterative(string[] source, string key, posicionArchivo commit)
        {
            observer result = new observer();
            //int index = 0;

            foreach (string caracter in source)
            {
                if (string.Equals(caracter.Trim(), key))
                {
                    if (!result.flag)
                    {
                        result.flag = true;
                        
                        if (commit == posicionArchivo.FIRST)
                        {                            
                            result.cantidad++;
                            return result;
                        }
                    }

                    result.cantidad++;
                }
            }

            return result;
        }            
    }
/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>
/// 

    public struct observer
    {
        public int cantidad;
        public bool flag;
        public List<int> positions;
        public observer(bool flag):this()
        {
             this.positions = new List<int>();
            this.cls();
        }

        public void cls()
        {
            this.cantidad = 0;
            this.flag = false;
        }
    }

    public enum posicionArchivo {FIRST, ALL};
}


