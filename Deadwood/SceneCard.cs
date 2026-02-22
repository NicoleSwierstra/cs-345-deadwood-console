class SceneCard
{
    public string name;
    public string desc;
    public int budget;
    private Role[] roles;

    public SceneCard(string name, string desc, int budget, Role[] roles)
    {
        this.name = name;
        this.desc = desc;
        this.budget = budget;
        this.roles = roles;
    }

    public Role[] getRoles() {
        return roles;
    }

    public override string ToString()
    {
        return "{\"" +name + ": " + desc + "\"}";
    }
}