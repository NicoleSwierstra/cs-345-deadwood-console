class Deck {
    /* this might be dangerous idk */
    private static Dictionary<SceneCard, int> scene_ids;

    private SceneCard[] cards;
    static Random rand = new Random((int)DateTime.Now.Ticks);
    
    private Deck(SceneCard[] c) {
        cards = c;
    }

    public static Deck fromXML(string filepath) {
        return new Deck([]);
    }

    /* silly little shuffling function */
    public Deck shuffled() {
        rand.Shuffle(cards);
        return this;
    }

    public SceneCard dealTop() {
        //todo
        return null;
    }
}