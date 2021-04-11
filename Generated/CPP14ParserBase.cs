using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public abstract class CPP14ParserBase : Parser
{
	public CPP14ParserBase(ITokenStream input) : base(input)
	{
	}

	public CPP14ParserBase(ITokenStream input, TextWriter output, TextWriter errorOutput) : this(input)
	{
	}

	public bool PureSpecifierCheck()
	{
		return true;
	}

	public bool TypeNameCheck1()
	{
		// A type-name is part of the decl-specifier-seq only if preceded by only cv-qualifiers.
		// If this node is contained in decl-specifier-seq:
		//  return true if first in the seq;
		var text = this.Context.GetText();
        ParserRuleContext ctx = this.Context;
		ParserRuleContext child = null;
		for (; ; )
		{
			var ri = ctx.RuleIndex;
			var rctx = this.RuleContext;
			var rn = this.RuleNames[ri];
			if (rn == "declSpecifierSeq") break;
			var p = ctx.Parent;
			if (p == null) return true;
			child = ctx;
			ctx = (ParserRuleContext)p;
		}
		if (ctx == null) return true;
		if (ctx.GetChild(0) == child) return true;
		for (int i = 0; i < ctx.ChildCount-1; ++i)
        {
            Antlr4.Runtime.Tree.IParseTree c = ctx.GetChild(i);
			if (!IsCvQualifier(c)) return false;
        }
		return true;
	}

	bool IsCvQualifier(Antlr4.Runtime.Tree.IParseTree c)
    {
		// Walk tree. Verify one cv-qualifier.
		Stack<IParseTree> stack = new Stack<IParseTree>();
		stack.Push(c);
		while (stack.Any())
        {
			var x = stack.Pop();
			if (x is TerminalNodeImpl) continue;
			var y = (ParserRuleContext)x;
			if (this.RuleNames[y.RuleIndex] == "cvQualifier")
				return true;
			for (int j = y.ChildCount - 1; j >= 0; --j)
				stack.Push(y.GetChild(j));
        }
		return false;
    }
}