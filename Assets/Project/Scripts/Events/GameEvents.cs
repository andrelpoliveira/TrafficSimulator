using System;

public static class GameEvents 
{
    public static Action OnGameStart; // Evento de início do game
    public static Action OnPlayerDeath; // Evento de colisão do player com veículo
    public static Action OnPlayerWin; // Evento de travessia completa do player
    public static Action OnResetGame; // Evento de reset do game

    public static Action<float> OnTimeUpdated; // Evento de atualização do tempo de jogo
}
