namespace SmartBall
{
    class DemoAppTask
    {
        public int Position       { get; set; }
        public string? Code      { get; set; } // Если пусто, то текущий режим - "код"
        public string? Data { get; set; } // 1 и более символов, не считая пробельные
    }
}
