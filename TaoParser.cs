using System.Collections.Generic;

public class TaoParser
{
    public Tao parse(string str) {
        return tao(new Input(str));
    }
    Tao tao(Input input) {
        var tao = new Tao();
        while (true) {
            if (input.atBound()) return tao;
            var part = tree(input);
            if (part.tag == "other") {
                part = op(input);
                if (part.tag == "other") {
                    part = note(input);
                }
            }
            tao.push(part);
        }
    }
    Tagged tree(Input input) {
        if (input.at('[')) {
            input.next();
            input.bound(']');
            var tree = tao(input);
            input.unbound();
            input.next();
            return new Tree(tree);
        }
        return new Other();
    }
    Tagged op(Input input) {
        if (input.at('`')) {
            input.next();
            if (input.done()) input.error("op");
            return new Op(input.next());
        }
        return new Other();
    }
    Tagged note(Input input) {
        if (meta(input)) input.error("note");
        string note = "" + input.next();
        while (true) {
            if (meta(input) || input.done()) return new Note(note);
            note += input.next();
        }
    }
    bool meta(Input input) {
        return input.at('[') || input.at('`') || input.at(']');
    }
    public class Tagged {
        public string tag {get;}
        protected Tagged(string tag) {
            this.tag = tag;
        }
    }
    public class Tao: Tagged {
        public List<Tagged> parts = new List<Tagged>();
        public Tao(): base("tao") {}
        public void push(Tagged part) {
            parts.Add(part);
        }
        override public string ToString() {
            var str = "";
            foreach (Tagged p in parts) {
                str += p.ToString();
            }
            return str;
        }
    }
    public class Tree: Tagged {
        public Tao tao {get;}
        public Tree(Tao tao): base("tree") {
            this.tao = tao;
        }
        override public string ToString() {
            return "[" + tao + "]";
        }
    }
    public class Note: Tagged {
        public string note {get;}
        public Note(string note): base("note") {
            this.note = note;
        }
        override public string ToString() {
            return note;
        }
    }
    public class Op: Tagged {
        public char op {get;}
        public Op(char op): base("op") {
            this.op = op;
        }
        override public string ToString() {
            return "`" + op;
        }
    }
    public class Other: Tagged {
        public Other(): base("other") {}
    }
    class Input {
        int length;
        int position;
        string str;
        Stack<(int, char)> bounds = new Stack<(int, char)>();
        public Input(string str) {
            this.length = str.Length;
            this.position = 0;
            this.str = str;
        }
        public bool done() { return position >= length; }
        public bool at(char symbol) { return str[position] == symbol; }
        public char next() { return str[position++]; }
        public void error(string name) {
            throw new System.Exception("Error: malformed " + name + " at " + position);
        }
        public void bound(char symbol) { bounds.Push((position, symbol)); }
        public void unbound() { bounds.Pop(); }
        public bool atBound() {
            if (bounds.Count > 0) {
                var (position, symbol) = bounds.Peek();
                if (done()) throw new System.Exception(
                    "ERROR: since " + position + " expected \"" + symbol + "\" before end of input"
                );
                return at(symbol);
            }
            return done();
        }
    }
}