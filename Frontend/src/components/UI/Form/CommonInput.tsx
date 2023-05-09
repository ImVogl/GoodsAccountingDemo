import classes from './CommonInput.module.css'
import { FC, ReactElement } from 'react';

export interface IInputProps extends React.InputHTMLAttributes<HTMLInputElement>{
}

/// Компонента с общестпользуемой кнопкой.
export const CommonInput: FC<IInputProps> = ({ ...props }): ReactElement => {
    return (<input {...props} className={classes.commonInput} />);
}