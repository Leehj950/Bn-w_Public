using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player
{
    private string name;
    private string id;
    private string specis = null;

    public string Name { get { return name; } set { name = value; } }
    public string Id { get { return id; } set { id = value; } }
    public string Specis { get {  return specis; } set {  specis = value; } }
    
}
