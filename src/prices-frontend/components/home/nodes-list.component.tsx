import { Checkbox, Grid, ListItem, ListItemButton, Stack } from "@mui/material";
import { useEffect, useState } from "react";
import { PricingNode, Rtos } from "../../gql/codegen/graphql";
import { Node } from "./node.component";
import { Symbol } from "../ticker/symbol.component";
import { Price } from "../ticker/price.component";

type NodesListProps = {
    pricingNodes: Array<PricingNode>;
    selectedNodes: Array<PricingNode>;
    onNodeClick: (rtoId: Rtos, node: string) => void;
    onSelectionChange: (pricingNodes: Array<PricingNode>) => void;
    subscribeToPriceChanges: Function;
};

export const NodesList = (props: NodesListProps) => {
    const { pricingNodes, selectedNodes, onNodeClick, onSelectionChange, subscribeToPriceChanges } = props;
    useEffect(() => subscribeToPriceChanges(), [subscribeToPriceChanges]);

    const [checked, setChecked] = useState<PricingNode[]>(selectedNodes);

    const handleToggle = (value: PricingNode) => () => {
        const currentIndex = checked.indexOf(value);
        const newChecked = [...checked];

        if (currentIndex === -1) {
            newChecked.push(value);
        } else {
            newChecked.splice(currentIndex, 1);
        }

        setChecked(newChecked);
        onSelectionChange(newChecked);
    };

    return (
        <Grid container spacing={2}>
            {pricingNodes.map((pn) => {
                const labelId = `checkbox-list-secondary-label-${pn}`;
                return (
                    <Grid key={pn.id} item xs={6} sm={12}>
                        <ListItem
                            secondaryAction={
                                <Checkbox edge="end" onChange={handleToggle(pn)} checked={checked.indexOf(pn) !== -1} inputProps={{ "aria-labelledby": labelId }} />
                            }
                            disablePadding
                        >
                            <ListItemButton sx={{ display: { xs: "none", sm: "block" } }} onClick={() => onNodeClick(pn.regionalTransmissionOperatorId, pn.name)}>
                                <Node pricingNode={pn} />
                            </ListItemButton>
                            <ListItemButton sx={{ display: { xs: "block", sm: "none" } }} onClick={() => onNodeClick(pn.regionalTransmissionOperatorId, pn.name)}>
                                <Stack direction="column" spacing={1} minWidth={100}>
                                    <Symbol pricingNode={pn} />
                                    <Price value={pn.currentPrice} options={{ currency: "USD" }} />
                                </Stack>
                            </ListItemButton>
                        </ListItem>
                    </Grid>
                );
            })}
        </Grid>
    );
};
