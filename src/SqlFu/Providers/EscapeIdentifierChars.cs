namespace SqlFu.Providers
{
    public struct EscapeIdentifierChars
    {
        public char Start;
        public char End;

        public EscapeIdentifierChars(char start,char end)
        {
            Start = start;
            End = end;
        }   
    }
}