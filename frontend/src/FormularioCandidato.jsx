import { useState } from "react";
import { cadastrarCandidato, extrairPdf } from "./api";

const FORMULARIO_VAZIO = {
  nomeCompleto: "",
  email: "",
  telefone: "",
  areaInteresse: "",
  resumoProfissional: "",
};

export default function FormularioCandidato({ aoSalvar }) {
  // "Memória" da tela: quando um desses valores muda, a tela se redesenha
  const [dados, setDados] = useState(FORMULARIO_VAZIO);
  const [erros, setErros] = useState({});
  const [aviso, setAviso] = useState(null); // { tipo: "sucesso" ou "erro", texto }
  const [lendoPdf, setLendoPdf] = useState(false);
  const [salvando, setSalvando] = useState(false);

  // Chamada a cada letra digitada: copia os dados atuais e troca só o campo que mudou
  function mudarCampo(evento) {
    setDados({ ...dados, [evento.target.name]: evento.target.value });
  }

  // Chamada quando a pessoa escolhe um PDF
  async function escolherPdf(evento) {
    const campoArquivo = evento.target;
    const arquivo = campoArquivo.files[0];
    if (!arquivo) {
      return;
    }

    setLendoPdf(true);
    setAviso(null);
    const resultado = await extrairPdf(arquivo);
    setLendoPdf(false);

    if (resultado.sucesso) {
      // O que o PDF encontrou preenche o campo; o que não encontrou fica como estava
      setDados({
        ...dados,
        nomeCompleto: resultado.dados.nomeCompleto || dados.nomeCompleto,
        email: resultado.dados.email || dados.email,
        telefone: resultado.dados.telefone || dados.telefone,
      });
      setErros({});
      setAviso({ tipo: "sucesso", texto: resultado.mensagem });
    } else {
      setAviso({ tipo: "erro", texto: resultado.mensagem });
    }

    // Limpa o campo para a pessoa poder escolher o mesmo arquivo de novo
    campoArquivo.value = "";
  }

  // Chamada quando a pessoa clica em salvar
  async function salvar(evento) {
    evento.preventDefault(); // impede o navegador de recarregar a página
    setSalvando(true);
    setAviso(null);

    const resultado = await cadastrarCandidato(dados);
    setSalvando(false);

    if (resultado.sucesso) {
      setDados(FORMULARIO_VAZIO);
      setErros({});
      aoSalvar(resultado.mensagem);
    } else {
      setErros(resultado.erros);
      setAviso({ tipo: "erro", texto: resultado.mensagem });
    }
  }

  return (
    <div className="cartao">
      <h2>Novo candidato</h2>

      <label>Preencher a partir de um currículo em PDF (opcional)</label>
      <input type="file" accept="application/pdf" onChange={escolherPdf} disabled={lendoPdf} />
      {lendoPdf && <p>Lendo o PDF...</p>}

      {aviso && (
        <div className={aviso.tipo === "sucesso" ? "aviso aviso-sucesso" : "aviso aviso-erro"}>
          {aviso.texto}
        </div>
      )}

      {/* noValidate: quem valida é o backend, para as regras serem as mesmas nos dois caminhos */}
      <form onSubmit={salvar} noValidate>
        <label>Nome completo *</label>
        <input name="nomeCompleto" value={dados.nomeCompleto} onChange={mudarCampo} />
        {erros.nomecompleto && <span className="erro-campo">{erros.nomecompleto}</span>}

        <label>E-mail *</label>
        <input type="email" name="email" value={dados.email} onChange={mudarCampo} />
        {erros.email && <span className="erro-campo">{erros.email}</span>}

        <label>Telefone</label>
        <input name="telefone" value={dados.telefone} onChange={mudarCampo} />
        {erros.telefone && <span className="erro-campo">{erros.telefone}</span>}

        <label>Área ou cargo de interesse</label>
        <input name="areaInteresse" value={dados.areaInteresse} onChange={mudarCampo} />
        {erros.areainteresse && <span className="erro-campo">{erros.areainteresse}</span>}

        <label>Resumo profissional</label>
        <textarea
          name="resumoProfissional"
          rows="5"
          value={dados.resumoProfissional}
          onChange={mudarCampo}
        />
        {erros.resumoprofissional && (
          <span className="erro-campo">{erros.resumoprofissional}</span>
        )}

        <p>
          <button type="submit" disabled={salvando}>
            {salvando ? "Salvando..." : "Salvar candidato"}
          </button>
        </p>
      </form>
    </div>
  );
}