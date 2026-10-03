import { useState } from "react";
import FormularioCandidato from "./FormularioCandidato";
import ListaCandidatos from "./ListaCandidatos";
import DetalhesCandidato from "./DetalhesCandidato";
import "./App.css";

export default function App() {
  const [tela, setTela] = useState("cadastro"); // "cadastro", "lista" ou "detalhes"
  const [idSelecionado, setIdSelecionado] = useState(null);
  const [mensagemSucesso, setMensagemSucesso] = useState(null);

  // Depois de salvar, vai para a lista mostrando a mensagem de sucesso
  function aoSalvar(mensagem) {
    setMensagemSucesso(mensagem);
    setTela("lista");
  }

  function abrirCadastro() {
    setMensagemSucesso(null);
    setTela("cadastro");
  }

  function abrirLista() {
    setMensagemSucesso(null);
    setTela("lista");
  }

  function verDetalhes(id) {
    setIdSelecionado(id);
    setTela("detalhes");
  }

  return (
    <div className="pagina">
      <h1>Cadastro de currículos</h1>

      <nav>
        <button onClick={abrirCadastro}>Novo cadastro</button>
        <button onClick={abrirLista}>Candidatos</button>
      </nav>

      {mensagemSucesso && <div className="aviso aviso-sucesso">{mensagemSucesso}</div>}

      {tela === "cadastro" && <FormularioCandidato aoSalvar={aoSalvar} />}
      {tela === "lista" && <ListaCandidatos aoVerDetalhes={verDetalhes} />}
      {tela === "detalhes" && <DetalhesCandidato id={idSelecionado} aoVoltar={abrirLista} />}
    </div>
  );
}