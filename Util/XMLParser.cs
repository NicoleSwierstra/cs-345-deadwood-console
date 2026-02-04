/* Nicole Swierstra
 *
 * My way to make XML files more generic and not have 300 dependancies on everything 
 * Tbh tho is this even necessary in c#? java is pretty horrible but this seems fine tbh 
 */

using System.Xml;
public class XMLParser {
    public class XMLObj {
        public string tag;
        public Dictionary<string, string> attribs;
        public List<XMLObj> children;
        public string contents;

        public XMLObj() {
            attribs = new Dictionary<string, string>();
            children = new List<XMLObj>();
            contents = null;
        }
    }

    public static XMLObj ReadFile(string filepath) {
        XmlDocument xml = new XmlDocument();
        xml.Load(filepath);

        return transcribe(xml.DocumentElement);
    }

    static XMLObj transcribe(XmlElement node) {
        XMLObj obj = new XMLObj();
        obj.tag = node.Name;
        
        XmlAttributeCollection attribs = node.Attributes;
        for (int i = 0; i < attribs.Count; i++) {
            obj.attribs.Add(attribs[i].Name, attribs[i].Value);
        }

        XmlNodeList children = node.ChildNodes;
        for (int i = 0; i < children.Count; i++) {
            obj.children.Add(transcribe((XmlElement)children[i]));
        }
        obj.contents = node.InnerText;

        return obj;
    }
}
