namespace Deadwood;

class ConsoleDeck {
    private List<Role>[] cards;
    private string[] scene_names;
    static Random rand = new Random((int)DateTime.Now.Ticks);
    private int counter = 0;
    
    private ConsoleDeck(List<Role>[] c, string[] n) {
        cards = c;
        scene_names = n;
    }

    public static ConsoleDeck fromXML(string filepath) {
        XMLParser.XMLObj root = XMLParser.ReadFile(filepath);
        List<XMLParser.XMLObj> children = root.children;
        List<Role>[] cardArray = new List<Role>[children.Count];
        string[] nameArray = new string[children.Count];

        // get card elements and set fields
        for (int i = 0 ; i < children.Count; i++) {
            string name = children[i].attribs["name"];
            string desc = children[i].children.Find(x => x.tag == "scene").contents;

            // Get the card's parts
            List<XMLParser.XMLObj> parts = children[i].children.FindAll(x => x.tag == "part");
            List<Role> roles = new List<Role>();

            // For each part, build a Role
            foreach (XMLParser.XMLObj p in parts) {
                string partName = p.attribs["name"];           
                int level = int.Parse(p.attribs["level"]);     
                string line = p.children.Find(x => x.tag == "line").contents;
                Role r = new Role(partName, line, level);
                roles.Add(r);
            }
            // build SceneCard
            cardArray[i] = roles;
            nameArray[i] = name;
        } 

        return new ConsoleDeck(cardArray, nameArray);
    }

    public string getName(int card) {
        return scene_names[card]; 
    }

    public List<Role> getRoles(int card) {
        return cards[card];
    }
}