
namespace Deadwood;
class SceneCard
{
    private string name;
    private string desc;
    private int budget;
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

    public string getName() {
        return name;
    }

    public string getDesc() {
        return desc;
    }

    public int getBudget() {
        return budget;
    }
}