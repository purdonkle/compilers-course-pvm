const int
      noSym        =  0,
      EOFSym       =  1,
      intSym       =  2,    // ADDED: other symbols for language
      charSym      =  3,
      voidSym      =  4,
      numSym       =  5,
      identSym     =  6,
      lparenSym    =  7,
      rparenSym    =  8,
      lbrackSym    =  9,
      rbrackSym    =  10,
      commaSym     =  11,
      semicolonSym =  12;

static void GetSym() {
    // Scans for next sym from input
      while (ch > EOF && ch <= ' ') GetChar();
      StringBuilder symLex = new StringBuilder();
      int symKind = noSym;

      if ((Char.IsLetter(ch) || ch == '_')) {               // ADDED: symbol is int/char/void/ident
        do {
          symLex.Append(ch); GetChar();
        } while (Char.IsLetterOrDigit(ch) || ch == '_');
        if (symLex.ToString() == "int") {
          symKind = intSym;
        } else if (symLex.ToString() == "char") {
          symKind = charSym;
        } else if (symLex.ToString() == "void") {
          symKind = voidSym;
        } else {
          symKind = identSym;
        }
      } 
      else if (Char.IsDigit(ch)) {                          // ADDED: symbol is num
        do {
          symLex.Append(ch); GetChar();
        } while (Char.IsDigit(ch));
        symKind = numSym;
      }
      else {                                                // ADDED: symbol is '(', ')', '[', ']', ',', ';', noSym
        symLex.Append(ch);
        switch (ch) {
          case EOF:
            symLex = new StringBuilder("EOF");
            symKind = EOFSym; break;
          case '(':
            symKind = lparenSym; GetChar(); 
            break;
          case ')':
            symKind = rparenSym; GetChar();
            break;
          case '[':
            symKind = lbrackSym; GetChar();
            break;
          case ']':
            symKind = rbrackSym; GetChar();
            break;
          case ',':
            symKind = commaSym; GetChar();
            break;
          case ';':
            symKind = semicolonSym; GetChar();
            break;
          default:
            symKind = noSym; GetChar();
            break;
        }
      }

      sym = new Token(symKind, symLex.ToString());
    } // GetSym