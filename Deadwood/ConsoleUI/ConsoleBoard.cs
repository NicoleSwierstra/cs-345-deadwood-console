

namespace Deadwood;
class ConsoleBoard {
    private Dictionary<int, string> names;
    private List<Role>[] tile_extras;

    private ConsoleBoard(Dictionary<int, string> d, List<Role>[] e) {
        names = d;
        tile_extras = e;
    }

    public static ConsoleBoard fromXML(string filepath) {
        Dictionary<string, int> tilenums = new Dictionary<string, int>();

        XMLParser.XMLObj root = XMLParser.ReadFile(filepath);

        List<XMLParser.XMLObj> xmltiles = root.children.FindAll(
            x => x.tag.ToLower() == "set" || x.tag.ToLower() == "trailer" || x.tag.ToLower() == "office"
        );

        /* generate location names for all of the */
        for (int i = 0; i < xmltiles.Count; i++) {
            tilenums.Add(
                xmltiles[i].attribs.ContainsKey("name") ? xmltiles[i].attribs["name"] : xmltiles[i].tag, i
            );
        }

        List<int>[] n_neighbors = new List<int>[xmltiles.Count];
        List<Role>[] n_extras = new List<Role>[xmltiles.Count];
        int[] n_shots = new int[xmltiles.Count];
        for (int i = 0; i < xmltiles.Count; i++) {
            n_neighbors[i] = new List<int>();
            foreach (XMLParser.XMLObj n in xmltiles[i].children.Find(x => x.tag == "neighbors").children) {
                n_neighbors[i].Add(tilenums[n.attribs["name"]]);
            }
            n_extras[i] = new List<Role>();
            XMLParser.XMLObj parts_node = xmltiles[i].children.Find(x => x.tag == "parts");
            
            if (parts_node != null) {
                /* we know this must be a set */
                foreach (XMLParser.XMLObj n in parts_node.children) {
                    n_extras[i].Add(new Role(n.attribs["name"], n.children.Find(x => x.tag == "line").contents, int.Parse(n.attribs["level"])));
                }
                n_shots[i] = xmltiles[i].children.Find(x => x.tag == "takes").children.Count;
            } else {
                /* we know this must be a special tile */
                n_shots[i] = -1;
            }
        }

        var names = new Dictionary<int, string>();
        foreach (string s in tilenums.Keys){
            names.Add(tilenums[s], s);
        } 

        return new ConsoleBoard(names, n_extras);
    }
    
    public string getTileName(int tile){
        return names[tile];
    }

    public int getTileIdx(string tile_name) {
        foreach (int i in names.Keys) {
            if (tile_name.Contains(names[i])) return i;
        }
        return -1;
    }
}