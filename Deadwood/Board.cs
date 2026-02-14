class Board {
    private Tile[] tiles; //12 tiles
    private int trailer_location;
    private int office_location;

    private Board(Tile[] t, int t_loc, int o_loc) {
        tiles = t;
        trailer_location = t_loc;
        office_location = o_loc;
    }

    public static Board fromXML(string filepath) {
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

        int trailer_loc = -1, office_loc = -1;
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
                if (xmltiles[i].tag == "trailer") {
                    trailer_loc = i;
                } else if (xmltiles[i].tag == "office") {
                    office_loc = i;
                } else {
                    throw new Exception("Huh?");
                }
            }
        }

        Tile[] tiles = new Tile[xmltiles.Count];
        for (int i = 0; i < xmltiles.Count; i++) {
            tiles[i] = new Tile(i, n_neighbors[i].ToArray(), n_shots[i], n_extras[i].ToArray());
        }
        return new Board(tiles, trailer_loc, office_loc);
    }

    public int[] getAdjacent(int tile) {
        return tiles[tile].neighbors;
    }
}