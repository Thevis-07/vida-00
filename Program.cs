DemoVida.Rodar();


public enum FaseDeVida { Infancia, Adolescencia, Adulto }
public enum TipoEvento { Nascimento, Escola, Conquista, Trabalho, Projeto, Religiao, Relacao, Saude, MudancaCidade, Amadureceu, Presente, Acidente}

public class EventoDeVida
{
    public DateTime Data { get; }
    public TipoEvento Tipo { get; }
    public string Descricao { get; }

    public EventoDeVida(DateTime data, TipoEvento tipo, string descricao)
    {
        Data = data;
        Tipo = tipo;
        Descricao = descricao;
    }
}

public interface IFaseDeVida
{
    FaseDeVida Tipo { get; }
    IEnumerable<string> AoEntrar();                            
    string ProcessarEvento(EventoDeVida e, Pessoa pessoa);     
}

public sealed class Infancia : IFaseDeVida
{
    public FaseDeVida Tipo => FaseDeVida.Infancia;

    public IEnumerable<string> AoEntrar() =>
        new[] { "Curiosidade", "Conviver em grupo" };

    public string ProcessarEvento(EventoDeVida e, Pessoa pessoa) => e.Tipo switch
    {
        TipoEvento.Escola => "Infância: base afetiva e social na escola.",
        TipoEvento.Conquista => "Infância: pequenas vitórias fortalecem autoestima.",
        TipoEvento.Religiao => "Infância: primeiros símbolos e referências espirituais.",
        _ => "Infância: evento registrado."
    };
}

public sealed class Adolescencia : IFaseDeVida
{
    public FaseDeVida Tipo => FaseDeVida.Adolescencia;

    public IEnumerable<string> AoEntrar() =>
        new[] { "Autonomia", "Organização básica" };

    public string ProcessarEvento(EventoDeVida e, Pessoa pessoa) => e.Tipo switch
    {
        TipoEvento.Escola => "Adolescência: escolhas formativas mais conscientes.",
        TipoEvento.Projeto => "Adolescência: experimentação vocacional.",
        TipoEvento.Relacao => "Adolescência: vínculos e responsabilidade afetiva.",
        _ => "Adolescência: evento registrado."
    };
}

public sealed class Adulto : IFaseDeVida
{
    public FaseDeVida Tipo => FaseDeVida.Adulto;

    public IEnumerable<string> AoEntrar() =>
        new[] { "Gestão do tempo", "Responsabilidade" };

    public string ProcessarEvento(EventoDeVida e, Pessoa pessoa) => e.Tipo switch
    {
        TipoEvento.Trabalho => "Adulto: consolidação de carreira e entregas.",
        TipoEvento.MudancaCidade => "Adulto: mobilidade estratégica e rede profissional.",
        TipoEvento.Projeto => $"Adulto: projeto orienta decisões (\"{pessoa.ProjetoNatureza}\").",
        _ => "Adulto: evento registrado."
    };
}

public class Pessoa
{
    public string Nome { get; }
    public DateTime Nascimento { get; }
    public string ProjetoNatureza { get; private set; }

    private IFaseDeVida _faseAtual;

    public FaseDeVida Fase => _faseAtual.Tipo;
    public List<string> Habilidades { get; } = new();
    public List<string> Papeis { get; } = new();
    public List<EventoDeVida> LinhaDoTempo { get; } = new();

    public Pessoa(string nome, DateTime nascimento, string projetoInicial)
    {
        Nome = nome;
        Nascimento = nascimento;
        ProjetoNatureza = projetoInicial;

        _faseAtual = new Infancia();
        Habilidades.AddRange(_faseAtual.AoEntrar());
        Papeis.Add("Filho");
        LinhaDoTempo.Add(new EventoDeVida(nascimento, TipoEvento.Nascimento, "Nasci"));
    }

    public void AvancarFase(FaseDeVida nova, string motivo)
    {
        if (nova == _faseAtual.Tipo) return;

        _faseAtual = nova switch
        {
            FaseDeVida.Infancia => new Infancia(),
            FaseDeVida.Adolescencia => new Adolescencia(),
            FaseDeVida.Adulto => new Adulto(),
            _ => _faseAtual
        };

        LinhaDoTempo.Add(new EventoDeVida(DateTime.Now, TipoEvento.Amadureceu, $"Evoluiu para {Fase} ({motivo})"));
        foreach (var hab in _faseAtual.AoEntrar()) Add(Habilidades, hab);
    }

    public void DefinirProjeto(string novoProjeto)
    {
        ProjetoNatureza = novoProjeto;
        LinhaDoTempo.Add(new EventoDeVida(DateTime.Now, TipoEvento.Projeto, $"Projeto de natureza: {novoProjeto}"));
    }

    public void RegistrarEvento(EventoDeVida e)
    {
        LinhaDoTempo.Add(e);

        var reacao = _faseAtual.ProcessarEvento(e, this);
        Console.WriteLine(reacao);

        switch (e.Tipo)
        {
            case TipoEvento.Escola:
                Add(Habilidades, "Aprendizagem");
                break;
            case TipoEvento.Conquista:
                Add(Habilidades, "Autoconfiança");
                break;
            case TipoEvento.Trabalho:
                Add(Papeis, "Profissional");
                Add(Habilidades, "Cumprir prazos");
                break;
            case TipoEvento.Projeto:
                Add(Papeis, "Autor/Construtor");
                Add(Habilidades, "Visão de produto");
                break;
            case TipoEvento.Religiao:
                Add(Habilidades, "Dimensão espiritual");
                break;
            case TipoEvento.MudancaCidade:
                Add(Habilidades, "Adaptabilidade");
                break;
        }
    }

    public void ApresentarResumo()
    {
        Console.WriteLine($"\n=== {Nome.ToUpper()} ===");
        Console.WriteLine($"Fase atual: {Fase}");
        Console.WriteLine($"Projeto de natureza: {ProjetoNatureza}");
        Console.WriteLine($"\nPapéis: {string.Join(", ", Papeis)}");
        Console.WriteLine($"Habilidades ({Habilidades.Count}): {string.Join(", ", Habilidades)}");
        Console.WriteLine("\nLinha do tempo:");
        foreach (var e in LinhaDoTempo.OrderBy(x => x.Data))
            Console.WriteLine($"- {e.Data:yyyy-MM-dd} • idade: {CalculateAge(new DateTime(2005, 2, 21), e.Data)} • {e.Tipo}: {e.Descricao}");
        Console.WriteLine("=========================\n");
    }

    // helpers
    internal void AddHabilidade(string nome) => Add(Habilidades, nome);
    internal void AddPapel(string nome) => Add(Papeis, nome);

    private static void Add(List<string> lista, string valor)
    {
        if (!lista.Contains(valor, StringComparer.OrdinalIgnoreCase))
            lista.Add(valor);
    }

    private static string CalculateAge(DateTime birthDate, DateTime currentDate)
    {

        birthDate = new DateTime(2005, 2, 21);

        int years =  currentDate.Year - birthDate.Year;


        return $"{years} anos";
    }
}

public static class DemoVida
{
    public static void Rodar()
    {
        var pessoa = new Pessoa(
            nome: "Thevis Cardoso",
            nascimento: new DateTime(2005, 2, 21),
            projetoInicial: "404 - Not Found"
        );

        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2007, 1, 1), TipoEvento.Conquista, "Aprendi a falar"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2009, 3, 1), TipoEvento.Presente, "Ganhei minha primeira bike"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2009, 3, 1), TipoEvento.Acidente, "Cai um tombo feio de bike e fiquei 2 dias sem ir na aula"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2010, 3, 1), TipoEvento.Escola, "Entrada no fundamental"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2010, 3, 1), TipoEvento.Conquista, "Comecei a andar de bike sem rodinhas"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2015, 11, 15), TipoEvento.Escola, "Troca de escola (fundamental)"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2016, 11, 01), TipoEvento.Religiao, "Conclui a catequese"));


        pessoa.AvancarFase(FaseDeVida.Adolescencia, "Mais autonomia e escolhas");
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2017, 11, 01), TipoEvento.Religiao, "Conclui a Crisma"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2017, 10, 01), TipoEvento.Conquista, "Aprendi a dirigir carro sozinho"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2018, 1, 10), TipoEvento.Trabalho, "Comecei a trabalhar para terceiros, alem da familia"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2018, 1, 30), TipoEvento.Conquista, "Aprendi a trabalhar sozinho no trator"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2019, 3, 01), TipoEvento.Conquista, "Aprendi a andar de moto sozinho"));

        
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2019, 10, 10), TipoEvento.Projeto, "Vereador por 1 dia (vice-presidente da câmara)"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2019, 12, 10), TipoEvento.Escola, "Formatura do fundamental"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2020, 7, 10), TipoEvento.Conquista, "Compra da sonhada Titan Mix"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2021, 2, 15), TipoEvento.Conquista, "Compra meu notebook"));

        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2021, 6, 01), TipoEvento.Trabalho, "Comecei a trabalhar na Prefeitura de Formiga city"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2022, 12, 05), TipoEvento.Escola, "Formatura do ensino "));


        // Adulto
        pessoa.AvancarFase(FaseDeVida.Adulto, "Faculdade e responsabilidades");
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2023, 2, 19), TipoEvento.Projeto, "API de cadastro de produtos pra tentar vaga de estagio (consegui sem saber o que estava fazendo)"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2023, 2, 21), TipoEvento.MudancaCidade, "Mudança para o Recanto Maestro"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2023, 2, 23), TipoEvento.Trabalho, "Estágio backend"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2023, 09, 23), TipoEvento.Conquista, "Fiz CNH AB"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2024, 09, 11), TipoEvento.Conquista, "Comprei meu carro"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2024, 10, 15), TipoEvento.Projeto, "Comecei a empresa com o Yuri"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2025, 03, 28), TipoEvento.Projeto, "Comecei no JET"));
        pessoa.RegistrarEvento(new EventoDeVida(new DateTime(2025, 04, 01), TipoEvento.Projeto, "Assumi a vice precidencia do DA do curso"));



        pessoa.DefinirProjeto("Em partes descoberto, sou um bom solucionador de problemas");

        pessoa.ApresentarResumo();
    }
}
