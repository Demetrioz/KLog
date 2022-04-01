import React, { useState } from "react";
import { useSnackbar } from "notistack";
import moment from "moment";

import Button from "@mui/material/Button";
import Checkbox from "@mui/material/Checkbox";
import DateTimePicker from "@mui/lab/DateTimePicker";
import FormControl from "@mui/material/FormControl";
import IconButton from "@mui/material/IconButton";
import InputAdornment from "@mui/material/InputAdornment";
import InputLabel from "@mui/material/InputLabel";
import ListItemText from "@mui/material/ListItemText";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import TextField from "@mui/material/TextField";

import FilterIcon from "@mui/icons-material/FilterList";
import SearchIcon from "@mui/icons-material/SearchRounded";
import VisibilityIcon from "@mui/icons-material/Visibility";

function LogSearchBar(props) {
  const { enqueueSnackbar } = useSnackbar();

  const [begin, setBegin] = useState(moment().subtract(30, "days"));
  const [end, setEnd] = useState(moment());

  const [searchText, setSearchText] = useState("");

  const [searchAnchor, setFilterAnchor] = useState(null);
  const searchOpen = Boolean(searchAnchor);

  const [visibilityAnchor, setVisibilityAnchor] = useState(null);
  const visibilityOpen = Boolean(visibilityAnchor);

  const handleSearchClick = (event) => {
    setFilterAnchor(event.currentTarget);
  };

  const handleVisibilityClick = (event) => {
    setVisibilityAnchor(event.currentTarget);
  };

  const handleClose = () => {
    setFilterAnchor(null);
    setVisibilityAnchor(null);
  };

  const handleSearch = () => {
    if (props.onSearch) props.onSearch(begin, end, searchText);
  };

  const handleBeginChange = (newValue) => {
    setBegin(newValue);
  };

  const handleEndChange = (newValue) => {
    setEnd(newValue);
  };

  const handleTextChange = (event) => {
    setSearchText(event.target.value);
  };

  return (
    <div id="search_bar" style={{ display: "flex" }}>
      {props.showDatePickers && (
        <React.Fragment>
          <div id="begin_date" style={{ marginRight: "8px", width: "380px" }}>
            <DateTimePicker
              renderInput={(props) => <TextField margin="normal" {...props} />}
              label="Begin"
              value={begin}
              ampm={false}
              onChange={handleBeginChange}
            />
          </div>
          <div id="end_date" style={{ marginRight: "8px", width: "380px" }}>
            <DateTimePicker
              renderInput={(props) => <TextField margin="normal" {...props} />}
              label="End"
              value={end}
              ampm={false}
              onChange={handleEndChange}
            />
          </div>
        </React.Fragment>
      )}

      {props.selectOptions && (
        <div id="app_select" style={{ marginRight: "8px" }}>
          <FormControl margin="normal">
            <InputLabel id="selectLabel">{props.selectLabel}</InputLabel>
            <Select
              sx={{ width: "100px", minWidth: "220px" }}
              labelId={props.selectLabel}
              id={props.selectLabel}
              multiple
              renderValue={props.selectRender}
              value={props.selectValue}
              label="Applications"
              onChange={props.handleSelectChange}
            >
              {props.selectOptions.map((option, index) => {
                return (
                  <MenuItem key={option.id} value={index}>
                    <Checkbox checked={props.selectValue.includes(index)} />
                    <ListItemText primary={option.text} />
                  </MenuItem>
                );
              })}
            </Select>
          </FormControl>
        </div>
      )}

      <TextField
        id="search"
        variant="outlined"
        margin="normal"
        fullWidth
        onChange={handleTextChange}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchIcon />
            </InputAdornment>
          ),
          endAdornment: (
            <InputAdornment position="end">
              {props.searchOptions && (
                <React.Fragment>
                  <IconButton aria-label="filter" onClick={handleSearchClick}>
                    <FilterIcon />
                  </IconButton>
                  <Menu
                    anchorEl={searchAnchor}
                    open={searchOpen}
                    onClose={handleClose}
                  >
                    {props.searchOptions.map((option) => {
                      return (
                        <MenuItem
                          key={option.id}
                          onClick={() => option.handler(!option.value)}
                        >
                          <Checkbox checked={option.value} />
                          {option.text}
                        </MenuItem>
                      );
                    })}
                  </Menu>
                </React.Fragment>
              )}
              {props.visibilityOptions && (
                <React.Fragment>
                  <IconButton
                    aria-label="display"
                    onClick={handleVisibilityClick}
                  >
                    <VisibilityIcon />
                  </IconButton>
                  <Menu
                    anchorEl={visibilityAnchor}
                    open={visibilityOpen}
                    onClose={handleClose}
                  >
                    {props.visibilityOptions.map((option) => {
                      return (
                        <MenuItem
                          key={option.id}
                          onClick={() => option.handler(!option.value)}
                        >
                          <Checkbox checked={option.value} />
                          {option.text}
                        </MenuItem>
                      );
                    })}
                  </Menu>
                </React.Fragment>
              )}
            </InputAdornment>
          ),
        }}
      />

      {props.showButton && (
        <Button
          variant="contained"
          color="primary"
          onClick={handleSearch}
          sx={{
            marginTop: "16px",
            marginBottom: "8px",
            marginLeft: "8px",
            width: "200px",
          }}
        >
          Search
        </Button>
      )}
    </div>
  );
}

export default LogSearchBar;
