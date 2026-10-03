import { useEffect, useState } from "react";
import { listarCandidatos } from "./api";

export default function ListaCandidatos({ aoVerDetalhes }) {
  const [candidatos, setCandidatos] = useState([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState(null);

  // Roda uma vez, quando a tela abre: busca a lista no backend
  useEffect(() => {
    async function carregar() {
      try {
        const lista = await listarCandidatos();
        setCandidatos(lista);
      } catch (e) {
        setErro(e.message);
      }
      setCarregando(false);
    }
    carregar();
  }, []);

  return (
    <div className="cartao">
      <h2>Candidatos</h2>

      {carregando && <p>Carregando...</p>}
      {erro && <p className="aviso aviso-erro">{erro}</p>}
      {!carregando && !erro && candidatos.length === 0 && (
        <p>Nenhum candidato cadastrado ainda.</p>
      )}

      {candidatos.length > 0 && (
        <div className="tabela">
          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>E-mail</th>
                <th>Telefone</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {candidatos.map((c) => (
                <tr key={c.id}>
                  <td>{c.nomeCompleto}</td>
                  <td>{c.email}</td>
                  <td>{c.telefone || "-"}</td>
                  <td>
                    <button onClick={() => aoVerDetalhes(c.id)}>Ver detalhes</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}