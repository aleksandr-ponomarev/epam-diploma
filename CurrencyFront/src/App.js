import React, { useReducer, useState } from 'react';
import ServerSimpleTableComponent from "./components/server-simple-table";
import './App.css';

const columns = [
  {
    field: "Date",
    headerName: "Date",
  },
  {
    field: "Name",
    headerName: "Name",
  },
  {
    field: "Valuteid",
    headerName: "Valuteid",
  },
  {
    field: "Numcode",
    headerName: "Numcode",
  },
  {
    field: "Charcode",
    headerName: "Charcode",
  },
  {
    field: "Nominal",
    headerName: "Nominal",
  },
  {
    field: "Value",
    headerName: "Value",
  },
];

function App() {
  const [list, setList] = useState([]);
  const [id, setId] = useState("");
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [page, setPage] = useState(1);
  const [size, setSize] = useState(1);

  const { REACT_APP_API_ADDRESS } = process.env
  const apiRequest = `http://${REACT_APP_API_ADDRESS}/currencyapi/api/v1/quotes`;

  const loadData = (item) => {
    fetch(`${apiRequest}?startDate=${start}&endDate=${end}&valuteId=${id}&page=${item.page}&pageSize=${item.numberPerPage}`)
      .then((response) => response.json())
      .then((json) => setList(json));
    setPage(item.page);
    setSize(item.numberPerPage);
  };

  const handleSubmit = (item) => {
    item.preventDefault();
    fetch(`${apiRequest}?startDate=${item.target.startdate.value}&endDate=${item.target.enddate.value}&valuteId=${item.target.valuteid.value}&page=${page}&pageSize=${size}`)
      .then((response) => response.json())
      .then((json) => setList(json));
     setId(item.target.valuteid.value);
     setStart(item.target.startdate.value);
     setEnd(item.target.enddate.value);
 }

  function reloadDb() {
    fetch(`${apiRequest}`, { method: 'POST'});
}

  function flushDb() {
    fetch(`${apiRequest}`, { method: 'DELETE'});
}

  return(
    
    <div className="wrapper">
      <h1>Currency quotes table</h1>
      <p>
      <button onClick={reloadDb}>Reload Db</button>
      <button onClick={flushDb}>Flush Db</button>
      </p>
      <form onSubmit={handleSubmit}>
        <fieldset>
          <label>
            <p>StartDate</p>
            <input name="startdate"/>
          </label>
          <label>
            <p>EndDate</p>
            <input name="enddate"/>
          </label>
          <label>
            <p>ValuteId</p>
            <input name="valuteid"/>
          </label>
        </fieldset>
        <p><button type="submit">Submit range</button></p>
      </form>
      <p></p>
      <tbody class="tbody">
      <p>
      <ServerSimpleTableComponent
      columns={columns}
      list={list}
      onGetData={loadData}
      total={1000}
      />
      </p>
      </tbody>
      </div>
  )
}

export default App;