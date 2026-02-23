
namespace Deadwood;

class Deck {
    private Dictionary<SceneCard, int> ids;

    private SceneCard[] cards;
    static Random rand = new Random((int)DateTime.Now.Ticks);
    private int counter = 0;
    
    private Deck(SceneCard[] c) {
        cards = c;
        int i = 0;
        ids = new Dictionary<SceneCard, int>();
        foreach (SceneCard sc in c) {
            ids.Add(sc, i);
            i++;
        }
    }

    public static Deck fromXML(string filepath) {
        XMLParser.XMLObj root = XMLParser.ReadFile(filepath);
        List<XMLParser.XMLObj> children = root.children;
        SceneCard[] cardArray = new SceneCard[children.Count];

        // get card elements and set fields
        for (int i = 0; i < children.Count; i++) {
            string name = children[i].attribs["name"];
            string desc = children[i].children.Find(x => x.tag == "scene").contents;
            int budget = int.Parse(children[i].attribs["budget"]);

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
            SceneCard card = new SceneCard(name, desc, budget, roles.ToArray());
            cardArray[i] = card;
        } 

        return new Deck(cardArray);
    }

    public int idOf(SceneCard card) {
        return ids[card];
    }

    /* silly little shuffling function */
    public Deck shuffled() {
        rand.Shuffle(cards);
        return this;
    }

    public SceneCard dealTop() {
        return cards[counter++];
    }
}