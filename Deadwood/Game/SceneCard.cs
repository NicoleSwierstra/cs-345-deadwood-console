
namespace Deadwood;
class SceneCard
{
    public string name;
    public string desc;
    private Role[] roles;

    public SceneCard(string name, string desc, Role[] roles)
    {
        this.name = name;
        this.desc = desc;
        this.roles = roles;
    }

    public string toString()
    {
        return "{\"" +name + ": " + desc + "\"}";
    }
}