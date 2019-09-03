using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipperConsole
{
    enum GopherItemType : int
    {
        None = 0,
        TextDocument = '0',
        SubmenuDir = '1',
        Phonebook = '2',
        Error = '3',
        BinHexMac = '4',
        DosFile = '5',
        UuEncode = '6',
        Search = '7',
        Telnet = '8',
        BinaryFile = '9',
        AltMirror = '+',
        GifImage = 'g',
        ImageFile = 'I',
        Telnet3270 = 'T',
        // Gopher+
        HtmlFile = 'h',
        InfoMessage = 'i',
        SoundFile = 's',
        PngFile = 'p'
    }
}
