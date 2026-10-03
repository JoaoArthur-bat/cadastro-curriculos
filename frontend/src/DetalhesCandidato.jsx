import { useEffect, useState } from "react";
import { buscarCandidato } from "./api";

export default function DetalhesCandidato({ id, aoVoltar }) {
  const [candidato, setCandidato] = useState(null);
  const [erro, setErro] = useState(null);

  useEffect(() => {
    async function carregar() {
      try {
        const dados = await buscarCandidato(id);
        setCandidato(dados);
      } catch (e) {
        setErro(e.message);
      }
    }
    carregar();
  }, [id]);

  return (
    <div className="cartao">
      <h2>Detalhes do candidato</h2>

      {erro && <p className="aviso aviso-erro">{erro}</p>}
      {!candidato && !erro && <p>Carregando...</p>}

      {candidato && (
        <div>
          <p><strong>Nome:</strong> {candidato.nomeCompleto}</p>
          <p><strong>E-mail:</strong> {candidato.email}</p>
          <p><strong>Telefone:</strong> {candidato.telefone || "Não informado"}</p>
          <p><strong>Área ou cargo de interesse:</strong> {candidato.areaInteresse || "Não informado"}</p>
          <p><strong>Resumo profissional:</strong> {candidato.resumoProfissional || "Não informado"}</p>
          <p><strong>Cadastrado em:</strong> {new Date(candidato.criadoEm).toLocaleDateString("pt-BR")}</p>
        </div>
      )}

      <button className="secundario" onClick={aoVoltar}>Voltar para a lista</button>
    </div>
  );
}