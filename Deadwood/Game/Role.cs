
namespace Deadwood;

/* Kinda unnecessary here because the deadwood server doesn't really care about it's contents, only it's presence. */
class Role
{
    public readonly string name;
    public readonly string line;
    public readonly int rank;

    public Role(string name, string line, int rank) {
        this.name = name;
        this.line = line;
        this.rank = rank;
    }

    public override string ToString() {
        return "{\"" + name + "\", \"" + line + "\", " + rank + "}";
    }
}