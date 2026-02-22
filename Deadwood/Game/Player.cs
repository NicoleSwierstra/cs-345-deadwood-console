namespace Deadwood;

class Player
{
    private string name;
    private int rank;
    private int credits;
    private int dollars;
    private int rehearsal_tokens;
    private int location;
    private int active_role;    // no role: -1, off-card: 0 <= extras.Length - 1, on-card onwards
    private bool has_moved;

    public Player(string name)
    {
        this.name = name;
        this.rank = 1;
        this.credits = 0;
        this.dollars = 0;
        this.rehearsal_tokens = 0;
        this.location = 0; // Assuming starting location is 0
        this.active_role = -1; // No active role
        this.has_moved = false;
    }

    public bool upgrade(int new_rank, UpgradeType type, int cost)
    {
        ref int currency = ref type == UpgradeType.DOLLARS ? ref dollars : ref credits;
        if (new_rank > this.rank && currency >= cost && new_rank <= 6)
        {
            this.rank = new_rank;
            currency -= cost;
            return true;
        }
        return false;
    }

    public bool move(int new_location) 
    {
        if (has_moved) 
            return false;
        this.location = new_location;
        this.has_moved = true;
        return true;
    }

    public void incCredits() {
        credits++;
    }

    public void incTokens() {
        rehearsal_tokens++;
    }

    public void incDollars(int amount) {
        dollars += amount;
    }

    //reset fields for next day
    public void resetForDay() {
        active_role = -1;
        rehearsal_tokens = 0;
        has_moved = false;
    }
    
    /* everything that happens once a turn has ended */
    public void endTurn() {
        has_moved = false;
    }

    public void setRole(int role) {
        rehearsal_tokens = 0;
        active_role = role;
    }

    public void setLocation(int loc) {
        location = loc;
    }

    public int getTokens() {
        return rehearsal_tokens;
    }

    public string getName() {
        return name;
    }

    public int getLocation() {
        return location;
    }

    public int getRole() {
        return active_role;
    }

    public int getRank() {
        return rank;
    }

    public int getCredits() {
        return credits;
    }
    
    public int getDollars() {
        return dollars;
    }
}