namespace SmartBall
{
    class DemoAppTask
    {
        public string? Task      { get; set; }
        public int BallPos       { get; set; }
        public string? Code      { get; set; } // Если пусто, то текущий режим - "код"
        public string? RulerData { get; set; } // 1 и более символов, не считая пробельные
    }
}
